# PocketSeller

A Xamarin.Android-based mobile sales application designed for field sales representatives.

## Project Structure

The solution consists of three main projects:

- **pocketseller.core**: Core business logic implemented in .NET Standard.
- **pocketseller.droid**: Android UI implementation.
- **Acr.UserDialogs**: Library for user dialog implementation.

## Features

PocketSeller supports the following functionalities:

- Document management (orders, delivery notes, invoices).
- Customer management.
- Quotation creation and management.
- Stock management and inventory.
- Data import/export functionality.
- Image capture and processing.

## Technical Details

- **Framework**: Built with Xamarin.Android.
- **Architecture**: Implements MVVM pattern using MvvmCross.
- **Target Android Version**: 12.0.
- **API Communication**: Utilizes RestSharp.
- **Local Storage**: SQLite database.
- **Reporting**: Includes FlexCel for Excel report generation.

## Building the Project

1. Open `pocketseller.sln` in Visual Studio.
2. Restore NuGet packages.
3. Build the solution.
4. Deploy to an Android device or emulator.

## Development Configuration

The project includes two main configurations:

- **Debug**: For development with debugging symbols.
- **Release**: For production deployment.

## License

This project is licensed under the **Business Source License 1.1 (BSL 1.1)**. Under this license:

- The source code is available for personal and non-commercial use.
- Commercial use by companies requires a separate commercial license.
- After a specified change date, the code will be made available under the Apache 2.0 License.

For more details, refer to the [LICENSE](LICENSE) file.