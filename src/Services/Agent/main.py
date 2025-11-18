import asyncio
import logging
import sys

# discovery
import services.consumers.agent_start_consumer
from services.broker import app

logger: logging.Logger = logging.getLogger(__name__)


def configure_logging() -> None:
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s - %(name)s - %(levelname)s - %(message)s",
        datefmt="%Y-%m-%d %H:%M:%S",
    )
    logging.getLogger("httpx").setLevel(logging.WARNING)
    logging.getLogger("mcp.client.streamable_http").setLevel(logging.WARNING)


async def main() -> None:
    try:
        await app.run()
    except Exception as e:
        logger.critical("Fatal error in main application: %s", e, exc_info=True)
        sys.exit(1)


if __name__ == "__main__":
    configure_logging()
    asyncio.run(main=main())
