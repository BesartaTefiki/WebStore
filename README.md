# WebStore Project

WebStore is a full-stack e-commerce application designed to provide a complete online shopping experience, including product browsing, stock management, quick ordering, and role-based administration functionality.

## Project Overview 

The WebStore project consists of a modern ASP.NET Core Web API backend paired with a React.js frontend.

Users can browse a collection of clothing products such as shirts, hoodies, pants, jackets, and more. Each product contains attributes including brand, category, size, color, gender, stock level, and optional discounts.

When the application is launched, visitors can explore all available products displayed in a clean and responsive grid layout. Each product card displays pricing, attributes, and a real-time stock indicator that stays synchronized with backend data.

Authorized users (admin, advanced, and content managers) can log in to access an administration panel where they can create, edit, and delete products, manage sizes, colors, categories, and upload product images. The system automatically updates stock quantities when orders are placed.

Authentication is implemented through custom user and role management. Visitors may browse freely, while only authorized users can modify store content.

---

## Getting Started

These instructions will help you set up and run the WebStore locally for development and testing.

---

## Prerequisites

Before starting, ensure you have installed:

- .NET 8 SDK or later  
- Node.js (includes npm)  
- SQL Server / LocalDB  
- Postman (optional, for API testing)

---

## Installation

### 1. Backend Setup (ASP.NET Core API)

Navigate to your backend folder:

```bash
cd path/to/WebStore
