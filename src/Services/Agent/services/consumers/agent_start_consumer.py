import logging

from faststream.rabbit import ExchangeType, RabbitExchange, RabbitQueue

from contracts import AgentStartContract
from services import BasicAgent, broker

logger = logging.getLogger(__name__)

exchange = RabbitExchange(name="agent.tasks", type=ExchangeType.TOPIC, durable=True)
queue = RabbitQueue(name="agent.tasks", durable=True)


@broker.subscriber(exchange=exchange, queue=queue)
async def agent_start(data: AgentStartContract) -> None:
    agent = BasicAgent(
        bot_id=data.bot_id,
        chat_context=data.chat_context,
        referenced_message=data.referenced_message,
    )
    if data.stream:
        await agent.run_stream(data.prompt)
    else:
        await agent.run(data.prompt)
