# Individual Reflection – Ramya

## 1. My Role in the Team

During the project, I worked with different parts of the Scanly solution, including the API, Docker, Azure deployment, Bicep, Azure DevOps and documentation.

At the beginning of the project, I worked with the invoice endpoints in the ASP.NET Core API. I implemented and tested endpoints such as `POST /invoices`, `GET /invoices/{id}` and `GET /invoices`. I used Swagger to test the API locally and to understand how the different endpoints worked.

I also worked with Docker and tested that the application could run as a container. This helped me understand the difference between running the API directly with .NET and running the same application inside a Docker container.

Another important part of my work was Azure. I worked with Azure Container Registry, Container Apps and the deployment of the application. I also worked with the Bicep configuration and learned how our Azure resources could be created and configured from code instead of only through the Azure Portal.

I used Azure DevOps during the project for branches, commits, Pull Requests and Work Items. We worked with separate task branches and merged our work through `dev` before it reached `main`.

Towards the end of the project, I also worked with the economics analysis using Azure Pricing Calculator and updated the technical documentation with the final cost calculations.

---

## 2. The Most Difficult Part

For me, one of the most difficult parts was understanding how all the different parts of the project were connected.

At first, I understood the API, Docker and Azure as separate things. During the project, I started to understand the complete flow: the .NET application is built, Docker creates an image, the image is pushed to Azure Container Registry, and Azure Container Apps uses that image to run the application.

Azure permissions and collaboration were also challenging. We were working as a team and needed access to shared Azure resources. Understanding roles, permissions, Managed Identity and which resources were created by which part of the deployment took some time.

Another challenge was troubleshooting when something worked locally but did not work in Azure. I learned that a successful build does not automatically mean that the application will work after deployment. Configuration, environment variables, ports, permissions and connections between Azure services also have to be correct.

Working through these problems helped me understand cloud deployment much better than only reading about it.

---

## 3. What Do I Understand Now That I Did Not Understand Before?

Before this project, I had used Azure services in smaller assignments, but I did not fully understand how they could be connected into one automated solution.

I now understand the complete process better, from writing code locally to running the application in Azure. I understand how Docker creates a portable application image, why Azure Container Registry is needed to store that image and how Container Apps can run it.

I also understand Bicep better. Before this project, Infrastructure as Code was still quite theoretical for me. Now I understand why it is useful to define Azure resources in files such as `main.bicep` and use different parameter files for development and production.

I also learned more about CI/CD and why automated build, testing and deployment are useful. Instead of manually repeating all deployment steps, a pipeline can perform them in a consistent order.

Another thing I understand better now is Managed Identity and RBAC. The application does not always need usernames, passwords or Storage Account keys. Azure can give the application an identity and then give that identity permission to access another Azure resource.

---

## 4. What Would I Do Differently?

If I started the project again, I would spend more time at the beginning understanding the complete architecture before starting individual tasks.

During the project, I sometimes focused on completing one task without fully understanding how it would affect another part of the system. For example, an API change can affect Docker, deployment, configuration and testing.

I would also test changes more systematically after each step. I would first verify the application locally, then in Docker and finally in Azure. This would make it easier to identify where a problem was introduced.

For Azure, I would rely on Bicep earlier instead of checking or configuring too many things manually in the Azure Portal. The Portal was useful for learning and troubleshooting, but using Infrastructure as Code gives a clearer and more repeatable configuration.

I would also communicate earlier with the team about who is changing shared infrastructure and deployment files. This could reduce conflicts and make collaboration easier.

---

## 5. Architecture and Economy

If a customer asked me how much the Scanly solution costs, I would explain that the cost depends mainly on the number of invoices that are processed and the Azure resources that are running.

For our scenario, we used Azure Pricing Calculator with approximately **30 customers and 15,000 invoices per month**, assuming approximately **1 page per invoice**.

Our estimated monthly costs were:

| Resource | Estimated Cost per Month |
| --- | ---: |
| Azure Container Apps | approx. 113 SEK |
| Azure Container Registry | approx. 48 SEK |
| Azure Blob Storage | approx. 3 SEK |
| Azure AI Document Intelligence | approx. 1,333 SEK |
| **Total** | **approx. 1,500 SEK** |

This is approximately **0.10 SEK per analyzed invoice** at launch.

If the customer base triples from 30 to 90 customers, we estimate approximately 45,000 invoices per month and a total Azure cost of approximately **4,360 SEK per month**.

The largest cost in our calculation is Azure AI Document Intelligence because the cost increases when more pages are analyzed.

If traffic increases four times, Container Apps can scale by adding more replicas up to the configured maximum. However, I would also monitor Azure AI Document Intelligence because every invoice depends on this service for analysis.

For a real production system, I would monitor actual usage, response times and Azure costs before deciding the final scaling limits.

---

## 6. What I Learned From the Project

The most important thing I learned from this project is that cloud development is not only about writing application code.

The code, Docker image, cloud resources, permissions, CI/CD pipeline, monitoring and costs all need to work together.

I also became more comfortable using Azure DevOps and Git as part of a team. Working with task branches and Pull Requests showed me why it is important to keep changes organized instead of everyone working directly in the same branch.

The project gave me practical experience with ASP.NET Core, Docker, Azure Container Registry, Azure Container Apps, Blob Storage, Document Intelligence, Bicep and Azure DevOps.

I still have more to learn, especially about larger production environments, but after this project I have a much clearer understanding of how a .NET application can go from local development to a working cloud solution.