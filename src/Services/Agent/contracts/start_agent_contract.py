from pydantic import BaseModel

from contracts.chat_message import ChatMessage


class StartAgentContract(BaseModel):
    prompt: str
    referenced_message_content: str | None
    chat_history: list[ChatMessage]

    guild_id: int
    channel_id: int

    user_id: int
    user_name: str
    is_admin: bool
