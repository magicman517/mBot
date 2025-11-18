from pydantic import BaseModel


class ChatContext(BaseModel):
    guild_id: str
    channel_id: str
    user_id: str
    prompt_message_id: str | None = None
