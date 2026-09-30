

# REFLEKTION_Zara

During this project, I mainly worked with the Azure infrastructure and deployment part. My main tasks were related to Bicep,
Azure resources, Container Apps, deployment verification and testing. I also followed the work in Azure DevOps by using branches,
tasks and pull requests. At the end, I also tested the invoice flow and checked that the API worked as expected.

One of the most difficult parts for me was understanding how all the Azure services connect to each other. 
In the beginning, I saw Docker, ACR, Container Apps, Bicep, Managed Identity and RBAC as separate things. 
It was difficult to understand the full picture. 
During the project, I slowly understood the flow better. For example, the application is first built, then packaged as a Docker image, the image is pushed to ACR, and then Azure Container Apps runs that image. Bicep is used to create and configure the Azure resources automatically.

I also learned more about the invoice flow in the application. 
The client sends an invoice to the API, the API processes it with Azure Document Intelligence, and the result can then be stored and retrieved.
This helped me understand how the backend, cloud services and storage work together.

Another important thing I learned was the difference between Managed Identity and RBAC. Managed Identity gives the application an identity in Azure, while RBAC controls what that identity is allowed to do. Before this project, I did not clearly understand this difference.

If I did this project again, I would test the complete flow earlier and document every step from the beginning.
I think this would make troubleshooting easier and save time. I would also check environment variables, ports and Azure configuration earlier,
because small configuration problems can stop the whole application even if the code is correct.

The architecture used a .NET API, Azure Document Intelligence, Blob Storage, Docker, ACR and Azure Container Apps.
From a cost perspective, it is important not to create more Azure resources than necessary and to remove unused resources after testing. 
One important security risk is exposed secrets or giving too many permissions. For that reason, using Managed Identity, RBAC and least privilege is important.

Overall, this project helped me understand the full cloud workflow much better.
I now have a clearer picture of how code, containers, infrastructure, deployment, security and testing are connected in a real Azure project.