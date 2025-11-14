# Agent Service

AI Agent service for mBot - handles AI interactions and processes messages via RabbitMQ.

## Overview

This service is responsible for:
- Consuming agent task messages from RabbitMQ
- Processing AI agent requests with chat history
- Managing agent interactions with various AI providers

## Technology Stack

- **Python 3.12+**
- **FastStream**: Modern async messaging framework for RabbitMQ
- **Pydantic AI**: AI agent framework with support for multiple providers
- **RabbitMQ**: Message broker for inter-service communication

## Project Structure

```
.
├── contracts/          # Shared message contracts (Pydantic models)
│   ├── chat_message.py
│   └── start_agent_contract.py
├── services/           # Service layer
│   ├── broker.py       # RabbitMQ broker configuration
│   ├── consumers/      # Message consumers
│   └── producers/      # Message producers
├── main.py             # Application entry point
└── pyproject.toml      # Project configuration and dependencies
```

## Setup

### Prerequisites

- Python 3.12 or higher
- RabbitMQ instance (configured via environment variables)

### Installation

Using `uv` (recommended):
```bash
uv sync
```

Or using `pip`:
```bash
pip install -e .
```

### Development Dependencies

Install with development tools:
```bash
uv sync --dev
# or
pip install -e ".[dev]"
```

## Configuration

The service requires the following environment variable:

- `ConnectionStrings__rabbitmq`: RabbitMQ connection URL (e.g., `amqp://guest:guest@localhost:5672/`)

## Running

```bash
python main.py
```

## Development

### Code Quality

This project uses:
- **Ruff**: Fast Python linter and formatter
- **mypy**: Static type checker
- **pytest**: Testing framework

Run linting:
```bash
ruff check .
ruff format .
```

Run type checking:
```bash
mypy .
```

Run tests:
```bash
pytest
```

## Message Contracts

### StartAgentContract

Received when an agent interaction is requested:
- `prompt`: User's message/prompt
- `referenced_message_content`: Optional referenced message
- `chat_history`: List of previous chat messages
- `guild_id`, `channel_id`: Discord context
- `user_id`, `user_name`, `is_admin`: User information

## Architecture

The service uses a consumer-based architecture:
1. Consumers are auto-discovered from the `services.consumers` package
2. Each consumer subscribes to specific RabbitMQ queues/exchanges
3. Messages are processed asynchronously using FastStream

## License

See repository root for license information.
