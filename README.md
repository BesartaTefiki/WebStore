# WebStore

# WebStore Project

WebStore is a full-stack e-commerce application designed to provide a complete online shopping experience, including product browsing, stock management, quick ordering, and role-based administration functionality.

## Project Overview 

The WebStore project consists of a modern ASP.NET Core Web API backend paired with a React.js frontend. Users can browse a collection of clothing products—shirts, hoodies, pants, jackets, and more—each defined by attributes such as brand, category, size, color, gender, stock level, and optional discounts.

When the application is launched, visitors can immediately begin exploring the shop, where all available products are displayed in a clean, responsive grid layout. Each product card contains price, tags, and a real-time stock indicator synchronized with backend data.

Admins, advanced users, and content managers can log in to access an administrative panel enabling them to create, edit, and delete products, manage sizes, colors, categories, and upload product images. The system includes real-time quantity updates, ensuring stock decreases automatically when orders are placed.

Authentication is handled through custom user and role management. Visitors may browse freely, while only authorized users can modify store content.

---

## Getting Started

These instructions will help you set up and run the WebStore on your local machine for development and testing.

---

## Prerequisites

Before starting, ensure you have installed:

- .NET 8 SDK or later  
- Node.js (includes npm)  
- SQL Server / LocalDB  
- Postman (optional, for API testing)

---

## Installation

### Backend Setup (ASP.NET Core API)

Navigate to your backend folder and restore required packages:

    dotnet restore

Apply database migrations:

    dotnet ef database update

Run the backend:

    dotnet run

API will start on:

    http://localhost:5169
    https://localhost:7183

---

### Frontend Setup (React)

Navigate to the frontend:

    cd path/to/webstore-frontend

Install dependencies:

    npm install

Start the development server:

    npm run dev

---

## Running the Project

Once both the backend and frontend are running, open the React app (usually at):

    http://localhost:5173

The frontend will automatically connect to the API.

Use the seeded admin account to log in:

    username: admin
    password: admin123

---

## Built With

- ASP.NET Core Web API  
- Entity Framework Core  
- React.js  
- Vite  
- SQL Server / LocalDB  
- Postman (API testing)

