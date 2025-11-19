🗄️ Database Initialization Guide

Before running the application, you must set up the database.

✅ 1. Run Database Migrations (RECOMMENDED)

If the project uses Entity Framework Core, run:

dotnet ef database update


This will create all tables and relationships automatically based on the EF model.

Make sure the connection string in appsettings.json is correct before running migrations.

✅ 2. (Alternative) Run SQL Schema Scripts

If you prefer to create the database manually, go to:

/retail-e-commerce/sql-script/


Run the schema scripts in order:

createTables.sql → creates all tables

entityRelations.sql → foreign keys, indexes, constraints

✅ 3. Insert Initial Data

After the database schema exists, run:

/retail-e-commerce/sql-script/initData.sql


This script adds demo data (categories, products, variants, etc.).
It must not be run before migrations/schema scripts.

🚀 4. Run the Backend API
dotnet run


Backend runs at:

http://localhost:5288/

🔗 5. Connect the Frontend

Update lib/apiClient.ts:

baseURL: "http://localhost:5288/"


Then run:

npm run dev
