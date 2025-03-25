# Amazon Price Monitor

A .NET Worker Service that monitors Amazon product prices using [Oxylabs Scraping API](https://oxylabs.io/pages/amazon-scraper-api) and sends email notifications when prices drop below a specified threshold.

## Features

- Monitors Amazon product prices using ASIN (Amazon Standard Identification Number)
- Configurable price threshold and check interval
- Email notifications when prices drop below the threshold
- Smart price tracking to avoid duplicate notifications
- Active monitoring only during business hours (7 AM - 12 AM)
- Resilient HTTP client with retry policies and circuit breaker
- Detailed logging for monitoring and debugging

### Email Notifications

When the price drops below your threshold, you'll receive an email containing:
- Product name
- Current price
- Price threshold
- Direct link to the Amazon product page


## Prerequisites

- An Oxylabs account (username, password) with access to the Amazon Scraper API
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
  "ScrapeApiSettings": {
    "ApiBaseUrl": "https://realtime.oxylabs.io",
    "Username": "your-oxylabs-username",
    "Password": "your-oxylabs-password"
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

### Setting Up Oxylabs API

1. Sign up for an Oxylabs account at [oxylabs.io](https://oxylabs.io)
2. Subscribe to the Amazon Scraper API service
3. Get your API credentials from the Oxylabs dashboard
4. Store your credentials securely using one of these methods:

   a. **User Secrets (Recommended for Development)**:
   ```bash
   dotnet user-secrets set "ScrapeApiSettings:Username" "your-username"
   dotnet user-secrets set "ScrapeApiSettings:Password" "your-password"
   ```

   b. **Environment Variables**:
   ```bash
   export ScrapeApiSettings__USERNAME="your-username"
   export ScrapeApiSettings__PASSWORD="your-password"
   ```

### Setting Up Email Notifications

1. Choose an email service provider (Gmail, Outlook, or your own SMTP server)
2. For Gmail:
   - Enable 2-factor authentication
   - Generate an App Password (Google Account → Security → App Passwords)
   - Use the App Password as your SMTP password

3. Store your email credentials securely using one of these methods:

   a. **User Secrets (Recommended for Development)**:
   ```bash
   dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.gmail.com"
   dotnet user-secrets set "EmailSettings:SmtpPort" "587"
   dotnet user-secrets set "EmailSettings:SmtpUsername" "your-email@gmail.com"
   dotnet user-secrets set "EmailSettings:SmtpPassword" "your-app-password"
   dotnet user-secrets set "EmailSettings:FromEmail" "your-email@gmail.com"
   dotnet user-secrets set "EmailSettings:ToEmail" "recipient@example.com"
   dotnet user-secrets set "EmailSettings:EnableSsl" "true"
   ```

   b. **Environment Variables**:
   ```bash
   export EmailSettings__SmtpServer="smtp.gmail.com"
   export EmailSettings__SmtpPort="587"
   export EmailSettings__SmtpUsername="your-email@gmail.com"
   export EmailSettings__SmtpPassword="your-app-password"
   export EmailSettings__FromEmail="your-email@gmail.com"
   export EmailSettings__ToEmail="recipient@example.com"
   export EmailSettings__EnableSsl="true"
   ```

### Finding Your Product's ASIN

1. Go to the Amazon product page
2. The ASIN is in the URL: `https://www.amazon.com/dp/ASIN_HERE`
3. Or find it in the product details section of the page

## Installation

1. Clone the repository:
```bash
git clone https://github.com/kyurkchyan/PriceMonitor
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