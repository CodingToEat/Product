# Product API

Product API is a simple API built with .NET 8 and EF Core using an in-memory database. It supports basic CRUD operations for managing products.

## Features

- Create, Read, Update, and Delete products
- In-memory database for testing
- HTTP client for external discount service simulation
- Caching with LazyCache
- Logging with Serilog
- Deployed to Azure with Application Insights

## Prerequisites

- .NET 8 SDK
- Azure subscription

## Getting Started

1. Clone the repository:

   ```bash
   git clone https://github.com/codingtoeat/product.git
   ```

2. Restore the dependencies:

   ```bash
   dotnet restore
   ```

3. Run the application:

   ```bash
   dotnet run
   ```

4. Run the tests:

   ```bash
   dotnet test
   ```

## Configuration

### Azure Deployment

1. Create an Azure App Service and configure Application Insights.
2. Set the following environment variables in Azure App Service:
   - `APPINSIGHTS_INSTRUMENTATIONKEY`: Your Application Insights instrumentation key.

### Local Development

1. Set the `APPINSIGHTS_INSTRUMENTATIONKEY` environment variable in your local development environment.

## Usage

### Endpoints

- `GET /products/{id}`: Get product by ID
- `POST /products`: Create a new product
- `PUT /products/{id}`: Update an existing product
- `DELETE /products/{id}`: Delete a product

### Example Requests

#### Create a Product

   ```bash
   curl -X POST "https://yourapi.azurewebsites.net/products" -H "Content-Type: application/json" -d '{
     "name": "Sample Product",
     "status": 1,
     "stock": 100,
     "description": "This is a sample product.",
     "price": 19.99
   }'
   ```

#### Get a Product

   ```bash
   curl "https://yourapi.azurewebsites.net/products/{id}"
   ```

#### Update a Product

   ```bash
   curl -X PUT "https://yourapi.azurewebsites.net/products/{id}" -H "Content-Type: application/json" -d '{
     "name": "Updated Product",
     "status": 1,
     "stock": 150,
     "description": "This is an updated product.",
     "price": 29.99
   }'
   ```

#### Delete a Product

   ```bash
   curl -X DELETE "https://yourapi.azurewebsites.net/products/{id}"
   ```

## Logging

This application uses Serilog for logging. Logs are configured to output to the console and to a rolling file (`logs/log-.txt`).

## Monitoring

Application Insights is used for monitoring and logging in the Azure environment. Ensure that the `APPINSIGHTS_INSTRUMENTATIONKEY` environment variable is set.

## Why Azure?

Azure was chosen for the deployment of this application due to its robust set of features and capabilities, which include:

- **Scalability**: Azure provides auto-scaling options to handle increased load, ensuring the application can handle a large number of concurrent users.
- **Reliability**: Azure's infrastructure ensures high availability and reliability for the application.
- **Integration**: Azure's suite of services, such as Application Insights and Azure App Service, provide seamless integration for monitoring, logging, and deployment.
- **Security**: Azure offers a comprehensive set of security tools and best practices to protect the application and data.

## Configuring Scalability in Azure

To handle a specific number of users calling this endpoint, you can configure scaling rules in Azure App Service.

### Steps to Configure Auto-Scaling:

1. **Navigate to Your App Service**:
   - Go to the Azure portal and navigate to your App Service.

2. **Set Up Scaling Rules**:
   - Select the "Scale out (App Service plan)" option under the "Settings" section.
   - Click on "Add a rule" to configure a new scaling rule.

3. **Configure Scaling Based on Metrics**:
   - Choose a metric for scaling, such as CPU usage, memory usage, or request count.
   - Set the threshold values for when to scale out (increase instances) or scale in (decrease instances).
   - For example, you can set a rule to add an instance when CPU usage exceeds 70% for 5 minutes.

4. **Set Instance Limits**:
   - Configure the minimum and maximum number of instances to ensure that the app scales within desired limits.

5. **Save and Apply**:
   - Save the scaling rule and ensure it is applied to your App Service plan.

### Example of Auto-Scaling Rule:

- **Rule**: Add one instance when the average CPU usage is above 70% for 5 minutes.
- **Minimum Instances**: 1
- **Maximum Instances**: 10

### Monitoring and Alerts:

- **Set Up Alerts**: Use Azure Monitor to set up alerts for metrics such as CPU usage, memory usage, and request count.
- **Review Logs**: Regularly review logs and metrics in Application Insights to monitor the application's performance and adjust scaling rules as needed.

## Diagrams

### Infrastructure Diagram

![image](https://github.com/CodingToEat/Product/assets/143816640/fe4eb921-601a-4ce7-9571-8db231e1823a)

### Architecture Diagram

![image](https://github.com/CodingToEat/Product/assets/143816640/99dbeb24-8412-4397-943e-829d1444080e)


To generate these diagrams, you can use the PlantUML tool or any online PlantUML editor, such as [PlantText](https://www.planttext.com/) or [PlantUML QEditor](http://plantuml.com/qa).

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
