from pydantic import BaseModel

from contracts.common import ChatContext


class AgentChunkContract(BaseModel):
    chat_context: ChatContext
    content: str
