import logging

from faststream.rabbit import ExchangeType, RabbitExchange, RabbitQueue

from contracts import AgentErrorContract
from services import broker

logger = logging.getLogger(__name__)

exchange = RabbitExchange(name="agent.tasks", type=ExchangeType.TOPIC, durable=True)
queue = RabbitQueue(name="agent.tasks.errors", routing_key="agent.tasks.errors", durable=True)


async def publish_error(data: AgentErrorContract) -> None:
    await broker.publish(data, exchange=exchange, routing_key="agent.tasks.errors")
    logger.info("Published error to RabbitMQ")
