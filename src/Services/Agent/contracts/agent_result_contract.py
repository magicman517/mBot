from pydantic import BaseModel

from contracts.common import ChatContext


class AgentResultContract(BaseModel):
    chat_context: ChatContext
    content: str
    total_tokens: int
