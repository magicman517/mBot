from pydantic import BaseModel

from contracts.common import ChatContext, ChatMessage, ReferencedMessage


class AgentStartContract(BaseModel):
    stream: bool
    bot_id: str
    chat_context: ChatContext

    prompt: str
    referenced_message: ReferencedMessage | None
    chat_history: list[ChatMessage]
