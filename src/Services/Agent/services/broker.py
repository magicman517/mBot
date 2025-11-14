import logging
import os
from typing import Final

from faststream import FastStream
from faststream.rabbit import RabbitBroker

logger: logging.Logger = logging.getLogger(__name__)

RABBITMQ_ENV_VAR: Final[str] = "ConnectionStrings__rabbitmq"


def get_rabbitmq_url() -> str:
    url = os.getenv(RABBITMQ_ENV_VAR)
    if not url:
        raise ValueError("RabbitMQ connection URL not found in environment variables")
    logger.info("RabbitMQ URL configured successfully")
    return url


broker: RabbitBroker = RabbitBroker(url=get_rabbitmq_url())
app: FastStream = FastStream(broker)
