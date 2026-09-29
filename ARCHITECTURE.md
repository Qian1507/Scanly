# Teknisk reflektion

## 1. Container Apps

Vi valde Azure Container Apps eftersom Scanly är en containeriserad ASP.NET Core API som behöver köras i Azure utan att teamet behöver administrera ett helt Kubernetes-kluster. Container Apps passar projektets storlek och ger stöd för container-revisioner, inbyggd ingress, HTTP-baserad autoskalning och Managed Identity. I utvecklingsmiljön använder vi 1–2 repliker och i den avsedda produktionskonfigurationen 2–5 repliker. Begränsningen är att vi inte får samma kontroll över noder, nätverk, schemaläggning och Kubernetes-resurser som i AKS. Vi skulle välja AKS om systemet blev betydligt större och behövde avancerad Kubernetes-konfiguration, flera tjänster eller mer direkt kontroll över klustret.

## 2. CI/CD

När en ändring pushas till en task-branch skapas en Pull Request till `dev`, där koden granskas och CI kan bygga projektet och köra testerna. `dev` används för integration och `main` innehåller den stabila versionen; push eller merge till `main` startar både CI och CD. CI återställer .NET-beroenden, bygger API:t och testprojektet samt kör automatiserade tester med fake-tjänster för Azure. CD deployar först `bootstrap.bicep`, bygger Docker-imagen, taggar den med Azure DevOps Build ID och pushar den till Azure Container Registry. Därefter deployas `main.bicep`, Container App uppdateras och `/health` verifieras. Om build eller tester misslyckas avbryts pipeline-flödet och ingen ny deployment görs.

## 3. IaC

Vi använder Bicep för att definiera Azure-infrastrukturen i versionshanterad kod i stället för att konfigurera allt manuellt i Azure Portal. `bootstrap.bicep` hanterar Azure Container Registry och `main.bicep` hanterar bland annat Storage Account, Blob-container, Container Apps Environment, Container App, Managed Identity, RBAC och autoskalning. `dev.bicepparam` och `prod.bicepparam` innehåller miljöspecifika värden, till exempel miljönamn och antal repliker, medan samma template kan återanvändas. Parameters gör lösningen flexibel mellan miljöer och minskar behovet av duplicerad kod. Outputs kan användas för att lämna vidare skapade resursers namn, URL:er eller andra värden till senare deploymentsteg. Bicep-deploymenten är tänkt att vara idempotent, vilket innebär att samma deployment kan köras flera gånger utan att skapa onödiga dubbletter. Det är viktigt i CI/CD eftersom infrastrukturen kan deployas upprepade gånger och ändå ge ett förutsägbart och konsekvent resultat.

## 4. Säkerhet

Blob Storage nås genom `DefaultAzureCredential`, Container Appens systemtilldelade Managed Identity och rollen `Storage Blob Data Contributor`. Samma identitet får rollen `AcrPull` för att Container App ska kunna hämta Docker-imagen från Azure Container Registry. Storage Account keys och connection strings används inte av applikationen, vilket minskar risken med långlivade credentials. Azure AI Document Intelligence-nyckeln kommer från den kurslevererade resursen och hanteras genom lokala miljövariabler samt säkra Azure DevOps- eller Container App-konfigurationer. Känsliga värden får inte hårdkodas i källkod, Dockerfiles, parameterfiler eller lokala scripts som committas till Git. Om en nyckel hamnar i Git-historiken ska den omedelbart återkallas eller roteras, en ny nyckel ska skapas och den gamla hemligheten ska tas bort ur koden och vid behov ur historiken. En vanlig senare commit räcker inte eftersom värdet fortfarande kan finnas kvar i tidigare commits.

## 5. Ekonomi

Utifrån vårt scenario uppskattar vi den initiala Azure-kostnaden till cirka **[XX SEK per månad]**. Beräkningen baseras på Azure Pricing Calculator och inkluderar Azure Container Apps, Azure Container Registry, Blob Storage och Azure AI Document Intelligence. Den största kostnaden är **[resurs]**, som uppskattas till cirka **[XX SEK per månad]**, eftersom **[kort förklaring]**.

Om antalet kunder ökar till tre gånger dagens nivå uppskattar vi den totala kostnaden till cirka **[XX SEK per månad]**. Kostnadsökningen kommer främst från **[till exempel fler Document Intelligence-anrop och högre belastning på Container Apps]**.

Om trafiken ökar fyra gånger kan Container Apps skala ut upp till det konfigurerade maxantalet repliker. Den första sannolika flaskhalsen är **[resurs eller tjänst]**, eftersom **[kort förklaring]**.

Kostnadsuppskattningen bygger på Azure Pricing Calculator och våra antaganden om **[X fakturor per månad]**, **[X sidor per faktura]** och **[X API-anrop per månad]**. Produktionskonfigurationen är inte separat deployad i projektet, men har validerats med Bicep `what-if`.