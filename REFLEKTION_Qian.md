# Individuell reflektion

## 1. Min roll i teamet


Under sprinten arbetade jag främst med testning, Bicep, Azure DevOps, CI/CD och projektets tekniska dokumentation.

Jag tog ett större ansvar för CI och de automatiserade API-testerna. Där ingick bland annat att få restore, build och test att fungera stabilt i pipeline-flödet och att använda fake-tjänster så att testerna kunde köras utan beroende av riktiga Azure-anrop.

Jag arbetade också med Azure DevOps-strukturen, bland annat Work Items, Pull Requests och kopplingen mellan commits och tasks. Det gjorde det lättare att följa arbetet och hålla implementationen kopplad till respektive uppgift.

Inom Bicep arbetade jag med hur infrastrukturen skulle struktureras och återanvändas mellan development och production genom bland annat `main.bicep`, `bootstrap.bicep`, `dev.bicepparam` och `prod.bicepparam`.

CD och deployment utvecklades tillsammans i teamet, och jag deltog i arbetet med att få flödet från pipeline till ACR och Container Apps att fungera.

Jag skrev även `README.md` och deltog i arbetet med `ARCHITECTURE.md` och `RAPPORT.md`, framför allt för att få dokumentationen att spegla den lösning vi faktiskt byggde och uppgiftens krav.

## 2. Det svåraste momentet

Det svåraste var att få ihop hela deployment-flödet och förstå beroendena mellan resurserna. Ett exempel var att Container App behöver en Docker image från ACR, men imagen kan inte pushas innan ACR finns. Därför behövde deploymenten delas upp i två steg, där `bootstrap.bicep` skapar eller uppdaterar ACR först och `main.bicep` deployas senare.

Ett annat konkret problem var portkonfigurationen i Container Apps. Applikationen lyssnade på port 8080, men ingress och health probes var först konfigurerade för port 80. Det gjorde att revisionen inte blev healthy. När target port och probe-portarna ändrades till 8080 fungerade deploymenten.


## 3. Vad förstår jag nu som jag inte förstod innan?

I början såg jag Bicep mest som ett sätt att skapa Azure-resurser med kod i stället för att klicka i portalen, och jag trodde att det främst kördes separat. Under projektet förstod jag bättre hur Bicep faktiskt passar in i CD-flödet och kan köras automatiskt som en del av deploymenten.

Jag förstår nu bättre varför Infrastructure as Code är designat för att vara deklarativt och idempotent. Infrastrukturen beskrivs som ett önskat tillstånd som kan deployas flera gånger på ett förutsägbart sätt, i stället för att varje körning skapar nya resurser eller kräver manuella ändringar.

Det är särskilt viktigt i CI/CD, där samma deployment kan köras många gånger. Jag förstår också bättre varför parameterfiler används för olika miljöer i stället för att duplicera infrastrukturen, och varför deploymenten delades upp i `bootstrap.bicep` och `main.bicep`: ACR måste finnas innan Docker-imagen kan pushas.

## 4. Vad skulle jag göra annorlunda?

Om jag gjorde om sprinten skulle jag använda Bicep tidigare som huvudvägen för att skapa och konfigurera Azure-resurser, i stället för att först göra flera delar manuellt i Azure Portal.

I början behandlade vi flera resurser som separata tasks och skapade dem manuellt för att verifiera att de fungerade. Det gjorde att vissa steg senare behövde göras igen när samma konfiguration skulle överföras till Bicep.

Jag skulle därför börja med att definiera ACR, Storage, Container Apps, Managed Identity, RBAC och scaling i Bicep så tidigt som möjligt och sedan använda `what-if` för att verifiera förändringarna innan deployment.

Det skulle minska dubbelarbete, göra infrastrukturen mer konsekvent och göra det tydligare vilka ändringar som faktiskt hör till den versionshanterade lösningen.

## 5. Arkitektur och ekonomi

Om en kund frågar vad lösningen kostar per månad skulle jag utgå från Azure Pricing Calculator och den faktiska belastningen i scenariot. Vid lansering, med cirka 30 kunder och 15 000 fakturor per månad, uppskattas kostnaden till cirka **1 500 SEK per månad**. Om kundbasen tredubblas till 90 kunder uppskattas kostnaden till cirka **4 360 SEK per månad**. Det motsvarar ungefär 0,10 SEK per analyserad faktura vid lansering.

Den största kostnaden är **Azure AI Document Intelligence**, eftersom kostnaden huvudsakligen baseras på antalet analyserade sidor och därför ökar med antalet fakturor.

Jag skulle säga att lösningens mest sårbara del är beroendet av externa Azure-tjänster och korrekt konfiguration mellan dem. Om trafiken ökar fyra gånger kan Container Apps skala ut upp till sitt konfigurerade maxantal repliker, men Document Intelligence-anrop, replica-gränser och andra tjänstegränser kan bli flaskhalsar. Därför skulle jag följa belastning, svarstider och kostnad och justera skalningsgränserna när verklig användning blir känd. 