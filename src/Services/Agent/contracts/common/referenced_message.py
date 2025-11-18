from pydantic import BaseModel


class ReferencedMessage(BaseModel):
    id: str
    content: str
