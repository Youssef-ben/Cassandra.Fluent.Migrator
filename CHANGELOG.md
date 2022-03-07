# Cassandra Fluent Migrator

All notable changes to the `Cassandra Fluent Migrator` project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

### Added

- Added a new Script `bump-version.sh` to automatically upgrade the version.
- Added a new Github actions `Pack` and `Push` to automatically push to NuGet.

### Changed

- Updated to the latest version the `DataStax` library `CassandraCSharpDriver v3.17`.

### WIP

- Changed the target framework from `.netCore3.1` to `netStandard2.1`.
- Fixed a typo `Registred` to `Registered`.

## [v1.0.2] - [Date: 2020-12-20]

### Changed

- Started using a `CHANGLOG` log to document the changes mades to the project.
- Started using `Semantic Versioning` system.
- Updated to the latest version the `DataStax` library `CassandraCSharpDriver`.
- Updated the Microsoft libraries for all the projects.