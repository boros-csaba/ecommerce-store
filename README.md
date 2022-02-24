# Custom Ecommerce Store
This is a custom ASP.NET MVC Core ecommerce website built from scratch. The website is live and running under this address: https://www.elenora.hu (Shipping only to Hungary). The backend is build using ASP.NET MVC Core with a Postgres database. The website is hosted on Amazon AWS, it is containerized using Docker and Traefik. There is also an admin panel created with Angular. 

## Integrations with 3rd party apps
 - Barion payment processor (this is a payment processor similar to PayPal, popular in Europe)
 - Facebook Conversions API for sending purchase and other events to facebook for ads optimisations
 - SendGrid for sending automated emails for order status and marketing messages
 - GLS Shipping integration
 - Google analytics and GA4 integrations

## Features
### Automated rendered photo-realisting product images
The code for the automated jewelry renderer can be found in this repo: https://github.com/boros-csaba/3d-bracelet-render-image-generator
The bracelets are made from gemstone beads and charms. My wife comes up with the design for the bracelets and writes down the beads in a Google Sheet. An automated task picks up the changes and generates a rendered image of the bracelet and automatically uploads it to the store. Here is an example image of a bracelet:
![Bracelet](examples/bracelet.jpg)
### Responsive UI
The website is designed and implemented using SCSS and jQuery. The cart events are handled using AJAX so the entire page is not reloaded when an item is added or removed from the cart.
![Desktop](examples/website.jpg)
![Mobile](examples/mobile.JPG)
