# Custom Ecommerce Store
This is a custom ASP.NET MVC Core ecommerce website built from scratch. The website is live and running under this address: https://www.elenora.hu (Shipping only to Hungary). The backend is build using ASP.NET MVC Core with a Postgres database. The website is hosted on Amazon AWS, it is containerized using Docker and Traefik. There is also an admin panel created with Angular. 

## Integrations with 3rd party apps
 - Barion payment processor (this is a payment processor similar to PayPal, popular in Europe)
 - Facebook Conversions API for sending purchase and other events to facebook for ads optimisations
 - SendGrid for sending automated emails for order status and marketing messages
 - GLS Shipping integration
 - Google analytics and GA4 integrations

## Features
### Responsive UI
The website is designed and implemented using SCSS and jQuery. The cart events are handled using AJAX so the entire page is not reloaded when an item is added or removed from the cart.
![Desktop](examples/website.jpg)
![Mobile](examples/mobile.JPG)
