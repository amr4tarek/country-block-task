## Country IP Blocking System

A .NET Core API for managing country-based IP blocking, with support for permanent and temporary blocks, geolocation, and access logging.

### Project Overview

This project provides a robust API for managing IP blocking based on country codes. It allows administrators to:

- Block countries permanently or temporarily
- Look up IP geolocation information
- Log and review blocked access attempts
- Check if a specific IP address is blocked

### Architecture

The solution follows Clean Architecture principles and is organized into four projects:

- **Block.API**: Web API controllers and configuration
- **Block.Application**: Application services, DTOs, and interfaces
- **Block.Domain**: Domain entities
- **Block.Infrastructure**: Repository implementations and external services
