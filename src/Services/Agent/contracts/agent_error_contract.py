from pydantic import BaseModel

from contracts.common import ChatContext


class AgentErrorContract(BaseModel):
    chat_context: ChatContext
    message: str
