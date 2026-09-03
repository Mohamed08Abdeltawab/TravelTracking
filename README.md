# TravelTracking App

A Windows Forms desktop application for managing travel-agency client records.
The interface is primarily in Arabic and provides client CRUD operations, filtering,
country and visa-type selection, and profile-image management.

## Features

- List clients in a read-only data grid, ordered by newest client ID.
- Add, edit, and delete clients.
- Open a client for editing by double-clicking its row.
- Filter clients by ID, name, phone number, email, country, or visa type.
- Display the number of records currently shown.
- Store client images using GUID-based filenames in the application `Images` folder.
- Validate required fields including name, passport number, phone number, country, and visa type.
- Load countries and visa types from the database for use in dropdown lists.

Client records contain the client's name, passport number, country, visa type, email,
password, phone number, address, notes, image path, creation date, and update date.

## Technology stack

- C# and Windows Forms
- .NET Framework 4.8
- SQLite using `System.Data.SQLite` 2.0.3
- Entity Framework 6.5.1 references are included in the data-access project, but the
  implemented client, country, and visa-type operations use parameterized SQLite commands directly.
- Traditional MSBuild project format

## Solution structure

The solution is located at `TravelTracking/TravelTracking.slnx` and contains three projects:

```text
TravelTracking/TravelTracking.slnx
├── TravelTracking/TravelTracking/   # WinForms application and UI forms
├── Buisness/                        # Business models and operations
└── DataAccess/                      # SQLite connection and data-access methods
```

The project dependency flow is:

```text
TravelTracking → Buisness → DataAccess
```

The application starts in `Program.Main()` and opens `frmListClients`.

## Requirements

- Windows
- Visual Studio with .NET desktop development tools
- .NET Framework 4.8 Developer Pack
- NuGet package restore support

## Database setup

The application expects a SQLite database named `TravelTracking.db` in the application
runtime directory:

```text
<application output directory>/TravelTracking.db
```

The connection string is defined in `DataAccess/clsDataAccessSettings.cs` and uses the
application base directory. The database must contain these tables:

- `Clients`
- `Countries`
- `VisaTypes`

The `Clients` table is expected to provide these columns:

`Id`, `FullName`, `PassportNumber`, `CountryId`, `Email`, `Password`, `PhoneNumber`,
`Address`, `Notes`, `VisaTypeId`, `ImagePath`, `CreatedAt`, and `UpdatedAt`.

This repository does not currently include a database file, schema script, or seed-data
script. Provide a compatible database and country/visa-type data before running the app;
otherwise the lists may be empty or database operations may fail.

## Build and run

1. Open `TravelTracking/TravelTracking.slnx` in Visual Studio.
2. Restore the NuGet packages if Visual Studio does not restore them automatically.
3. Set the `TravelTracking` project as the startup project.
4. Choose `Any CPU` or `x86` to match the available SQLite native dependencies.
5. Build the solution.
6. Run the application.

The executable is produced in the selected project's `bin/Debug` or `bin/Release`
directory. On startup, the client list form is displayed.

## Client workflow

### Add or edit a client

Use the add action to create a record, or select a row and double-click it to edit.
Choose an image from a JPG, JPEG, PNG, GIF, or BMP file. Selected images are copied to
the runtime `Images` directory and renamed with a generated GUID.

### Filter clients

Select a filter type from the filter dropdown. Text filters update as you type; country
and visa-type filters use dropdown selections. The record count updates after filtering.

### Delete a client

Select a client and use the delete action. After confirmation, the database record is
deleted and its stored image is removed when possible.

## Project files

- `TravelTracking/TravelTracking/frmListClients.cs` — client list, filtering, and actions
- `TravelTracking/TravelTracking/frmAddUpdateClient.cs` — add/edit form and validation
- `Buisness/clsClient.cs` — client business model and save/find/delete operations
- `DataAccess/clsClientData.cs` — client SQLite queries
- `DataAccess/clsCountryData.cs` — country queries
- `DataAccess/clsVisaTypesData.cs` — visa-type queries
- `TravelTracking/TravelTracking/Global Classes/clsUtil.cs` — image and folder utilities
