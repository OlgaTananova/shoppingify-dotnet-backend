# Shoppingify &mdash; Your Shopping Companion

[Try it out!](https://olgatananova.github.io/shoppingify)


## Overview

**Shoppingify** is a dynamic shopping list app designed to ease your daily shopping routine. Build a personalized database of items, craft lists, track expenses, and review your shopping history and stats.

## Key Features

- **Personal Account Creation**: Securely create and log into your personal account.
    <details>
    <summary><i><b>Demo</b></i></summary>

    ![](https://github.com/OlgaTananova/shoppingify/blob/develope/src/images/demo1.gif) 

    ![](https://github.com/OlgaTananova/shoppingify/blob/develope/src/images/demo2.gif)
    </details>

- **Customizable Shopping Database**: Tailor your shopping item database to your needs by adding, editing, or removing items and catgories.
    <details>
    <summary><i><b>Demo</b></i></summary>
  
    ![](https://github.com/OlgaTananova/shoppingify/blob/develope/src/images/demo3.gif)
    ![](https://github.com/OlgaTananova/shoppingify/blob/develope/src/images/demo4.gif)
    </details>

- **Flexible Shopping Lists**: Generate and modify shopping lists with ease, adjust item quantities, units, and prices as needed, and transform your lists into detailed grocery bills.
    <details>
    <summary><i><b>Demo</b></i></summary>
  
    ![](https://github.com/OlgaTananova/shoppingify/blob/develope/src/images/demo5.gif)    
    </details>  
    
- **Bill Upload & Integration**: Shoppingify allows you to upload your bills in PDF format, integrating them with your current shopping list or uploading them separately. Azure Intelligence Services (OCR Technology) and OpenAI API parse and analyse the content of your uploaded bills. You can manually edit these bills prior to integration or upload to ensure optimal accuracy and precision in your data.
    <details>
    <summary><i><b>Demo</b></i></summary>
    
    ![](https://github.com/OlgaTananova/shoppingify/blob/develope/src/images/demo7.gif)
    </details>
- **Archivable Shopping Lists and Shopping Analytics**: You can archive your shopping lists to maintain a thorough history of your shopping, keeping track of past purchases and trends. You can access detailed statistics related to your shopping history. This includes a total of purchased items grouped by month, year, and category, as well as your top-purchased items and categories, giving you an understanding of your shopping habits and expenditure.
    <details>
    <summary><i><b>Demo</b></i></summary>
    
    ![](https://github.com/OlgaTananova/shoppingify/blob/develope/src/images/demo6.gif)
    </details>

## Key Technologies
- **Language & Frameworks**: **C#** with **ASP.NET Core**
- **Database Management**: **Azure SQL Database** integrated with **Entity Framework** for object-relational mapping.
- **Authentication & Authorization**: **Identity Framework** for managing user registration, authentication, and authorization processes, enhancing security and user management.
- **PDF Processing**: **AzureAI.FormRecognizer** and **iText7** for parsing and processing uploaded bills in PDF format, extracting essential data to interact with shopping lists.
- **Logging & Error Handling**: **Serilog** for logging, aiding in the effective monitoring and troubleshooting of the application.
- **Cloud Services & Deployment**: **Azure**, including **Azure App Service**, providing a scalable, secure, and managed hosting environment.
- **External API Integration**: **OpenAI API** for advanced data processing and analysis, enhancing the application's capabilities.

## Backend Functionalities:

- #### User Management:
    C#, ASP.NET Core, and Identity Framework for secure user registration, login, logout, and password management.

- #### Category & Product Management:

    C#, ASP.NET Core, and Entity Framework to enable users to create, organize, and manage categories and products.

-   #### Shopping List Management:

    C#, ASP.NET Core, and Entity Framework for creating, updating, and deleting shopping lists, allowing users to efficiently manage their shopping needs and track expenses.

- #### Bill Upload & Processing:

    AzureAI.FormRecognizer and iText7 for uploading bills in PDF format, parsing, and merging them with shopping lists. The OpenAI API enhances this process by analyzing the extracted data and returning structured information in JSON format.

- #### Security & Integrity:

     All requests conform to RESTful principles and are validated by ASP.NET Core, safeguarding the application's security and data integrity.Incorporates JWT authentication to protect routes, ensuring that only authorized users can access sensitive functionalities.

- #### Error Handling & Logging:

    Centralized error handling mechanism combined with Serilog logging to capture and log errors.

- #### Deployment
    Deployed on Azure using Azure App Service.

## Technologies (front-end part)
   React, TypeScript, Redux (Redux Toolkit), React Router, JWT, EsLint, GitHub Pages

[Please, refer to the front-end part repository for more details](https://github.com/OlgaTananova/shoppingify)

The application is deployed on GitHub Pages: [Link to the app](https://olgatananova.github.io/shoppingify)

If you want to get to know more about the application features, please refer to the demo [Link to the demo](https://scribehow.com/shared/How_to_use_Shoppingify_to_Manage_Shopping_Lists_and_Expenses__mXdmP1zCRomx_SX0_ys_CQ)

