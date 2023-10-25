# Cassandra Fluent Migrator

All notable changes to the `Cassandra Fluent Migrator` project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [v1.0.4] - [Date: 2023-10-24]

### Added

- Added a new Example project using the Net7

### Changed

- Updated to the latest version of the `DataStax` library `CassandraCSharpDriver v3.19.x`.
- Updated the packages to use the latest version
- Updated the test project to the latest framework version
- Refactored the test project and now start the cassandra container automatically.
- Updated the documentation of some methods.

## [v1.0.3] - [Date: 2022-03-06]

### WIP

- Changed the target framework from `.netCore3.1` to `netStandard2.1`.
- Fixed a typo `Registred` to `Registered`.

### Added

- Added a new Script `bump-version.sh` to automatically upgrade the version.
- Added a new Github actions `Pack` and `Push` to automatically push to NuGet.

### Changed

- Updated to the latest version the `DataStax` library `CassandraCSharpDriver v3.17`.
- Fixed some typos errors.

## [v1.0.2] - [Date: 2020-12-20]

### Changed

- Started using a `CHANGELOG` log to document the changes made to the project.
- Started using `Semantic Versioning` system.
- Updated to the latest version the `DataStax` library `CassandraCSharpDriver`.
- Updated the Microsoft libraries for all the projects.
