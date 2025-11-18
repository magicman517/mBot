from faststream.rabbit import ExchangeType, RabbitExchange, RabbitQueue

from contracts import AgentResultContract
from services import broker

exchange = RabbitExchange(name="agent.tasks", type=ExchangeType.TOPIC, durable=True)
queue = RabbitQueue(name="agent.tasks.results", routing_key="agent.tasks.results", durable=True)


async def publish_result(data: AgentResultContract) -> None:
    await broker.publish(data, exchange=exchange, routing_key="agent.tasks.results")
