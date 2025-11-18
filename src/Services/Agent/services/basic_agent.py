import logging
import os
from typing import Final

from pydantic_ai import (
    Agent,
    AgentRunResultEvent,
    BuiltinToolCallPart,
    PartStartEvent,
    ThinkingPart,
    ToolCallPart,
)
from pydantic_ai.common_tools.duckduckgo import duckduckgo_search_tool
from pydantic_ai.mcp import MCPServerStreamableHTTP
from pydantic_ai.models.google import GoogleModel, GoogleModelSettings

from contracts import AgentChunkContract, AgentErrorContract, AgentResultContract
from contracts.common import ChatContext, ReferencedMessage
from services.publishers import publish_chunk, publish_error, publish_result
from utils import get_instruction

MCP_URL_ENV_VAR: Final[str] = "ConnectionStrings__mcp"
MODEL_NAME: Final[str] = "gemini-flash-latest"

logger = logging.getLogger(__name__)


class BasicAgent:
    def __init__(
        self,
        *,
        bot_id: str,
        chat_context: ChatContext,
        referenced_message: ReferencedMessage | None = None,
    ) -> None:
        self._chat_context = chat_context
        self._bot_id = bot_id
        self._referenced_message = referenced_message

        self._agent = self._create_agent()

    def _create_agent(self) -> Agent:
        mcp_server = self._get_mcp_server()

        instruction = get_instruction(
            bot_id=self._bot_id,
            guild_id=self._chat_context.guild_id,
            channel_id=self._chat_context.channel_id,
            user_id=self._chat_context.user_id,
            referenced_message=self._referenced_message,
        )

        return Agent(
            model=GoogleModel(MODEL_NAME),
            tools=[duckduckgo_search_tool()],
            toolsets=[mcp_server],
            system_prompt=instruction,
            model_settings=GoogleModelSettings(google_thinking_config={"include_thoughts": True}),
        )

    def _get_mcp_server(self) -> MCPServerStreamableHTTP:
        url = os.getenv(MCP_URL_ENV_VAR)
        if not url:
            raise ValueError(f"Environment variable '{MCP_URL_ENV_VAR}' not found.")
        return MCPServerStreamableHTTP(url)

    async def run(self, prompt: str) -> None:
        try:
            result = await self._agent.run(prompt)
            await publish_result(
                AgentResultContract(
                    chat_context=self._chat_context,
                    content=result.output,
                    total_tokens=result.response.usage.total_tokens,
                )
            )
        except Exception as e:
            await self._handle_error(e)

    async def run_stream(self, prompt: str) -> None:
        try:
            async for event in self._agent.run_stream_events(prompt):
                await self._handle_stream_event(event)
        except Exception as e:
            await self._handle_error(e)

    async def _handle_stream_event(self, event) -> None:
        if isinstance(event, PartStartEvent):
            await self._handle_part_start(event.part)
            return

        if isinstance(event, AgentRunResultEvent):
            await publish_result(
                AgentResultContract(
                    chat_context=self._chat_context,
                    content=event.result.output,
                    total_tokens=event.result.response.usage.total_tokens,
                )
            )

    async def _handle_part_start(self, part) -> None:
        content = None

        if isinstance(part, ThinkingPart):
            content = "`🧠 Thinking`"
        elif isinstance(part, ToolCallPart | BuiltinToolCallPart):
            content = f"`🛠️ Used {part.tool_name!r}`"

        if content:
            await publish_chunk(
                AgentChunkContract(
                    chat_context=self._chat_context,
                    content=content,
                )
            )

    async def _handle_error(self, e: Exception) -> None:
        logger.error(f"Unexpected error in BasicAgent: {e}", exc_info=True)

        await publish_error(
            AgentErrorContract(
                chat_context=self._chat_context,
                message=f"Unexpected error\n\n-# {e}",
            )
        )
