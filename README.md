# Amazon Price Monitor

A .NET Worker Service that monitors Amazon product prices and sends email notifications when prices drop below a specified threshold.

## Features

- Monitors Amazon product prices using ASIN (Amazon Standard Identification Number)
- Configurable price threshold and check interval
- Email notifications when prices drop below the threshold
- Smart price tracking to avoid duplicate notifications
- Active monitoring only during business hours (7 AM - 12 AM)
- Resilient HTTP client with retry policies and circuit breaker
- Detailed logging for monitoring and debugging

## Prerequisites

- .NET 9.0 SDK or later
- An Amazon ASIN for the product you want to monitor
- SMTP server credentials for sending email notifications

## Configuration

The application uses `appsettings.json` for configuration. Create or modify the following sections:

```json
{
  "PriceMonitorSettings": {
    "Asin": "B0C9J5R4L7",  // Your Amazon product ASIN
    "PriceThreshold": 875.89,  // Your desired price threshold
    "CheckIntervalMinutes": 60  // How often to check the price
  },
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@example.com",
    "SmtpPassword": "your-smtp-password",
    "FromEmail": "your-email@example.com",
    "ToEmail": "recipient@example.com",
    "EnableSsl": true
  }
}
```

### Finding Your Product's ASIN

1. Go to the Amazon product page
2. The ASIN is in the URL: `https://www.amazon.com/dp/ASIN_HERE`
3. Or find it in the product details section of the page

## Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/PriceMonitor.git
cd PriceMonitor
```

2. Build the project:
```bash
dotnet build
```

3. Run the service:
```bash
dotnet run
```

## Email Notifications

When the price drops below your threshold, you'll receive an email containing:
- Product name
- Current price
- Price threshold
- Direct link to the Amazon product page
