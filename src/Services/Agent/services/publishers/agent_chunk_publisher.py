from faststream.rabbit import ExchangeType, RabbitExchange, RabbitQueue

from contracts import AgentChunkContract
from services import broker

exchange = RabbitExchange(name="agent.tasks", type=ExchangeType.TOPIC, durable=True)
queue = RabbitQueue(name="agent.tasks.chunks", routing_key="agent.tasks.chunks", durable=True)


async def publish_chunk(data: AgentChunkContract) -> None:
    await broker.publish(data, exchange=exchange, routing_key="agent.tasks.chunks")
