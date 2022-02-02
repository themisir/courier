<center>
  <img src="./Courier/wwwroot/assets/logo.svg" height="70" alt="courier logo" />
</center>

---

<center>Package manager for private Dart & Flutter projects.</center>

## Features

- **Secure** - Web app is protected with hashed and salted password authentication (OpenID authentication support is planned to be implemented)
- **Customizable** - The project is fully open source with restrictive license. So you can modify core parts to fit your organization's needs
- **Configurable** - You can configure every single configurable part of the project from using different storage solutions to using different database sources.
- **Access Controls** - You can define individual access controls for individual packages in the platform. (Global and role based access controls planned to be implemented)

## Installation

### Docker (suggested)

Pull official Courier image from GitHub Container registry and run it inside docker or any OCI supported container runtime.

```sh
docker pull ghcr.io/themisir/courier:main
```

### Standalone

Standalone installation requires .NET 6 ASP.NET Core SDK to be installed on target machine.

- [.NET 6.0 Installation](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

If you have .NET 6.0 SDK already installed you can clone source repository, use build script to build it and start using.

```sh
# Clone
git clone https://github.com/themisir/courier.git

# Build
cd courier
tool/build.sh

# Run
cd publish
dotnet Courier.dll
```

> Note: .NET SDK required for building source code. If you have prebuilt binaries you'll only need to install .NET Runtime which is slightly lightweight installation of .NET.

## Configuration

Application configuration could be done either using environment variables or `appsettings.Production.json` file. Configuration provided via environment variables will override json configurations.

| Config key                     | Description                                       |
| :----------------------------- | :------------------------------------------------ |
| `Archives.Type`                | Archive storage type (`local`, `s3`)              |
| `Archives.Local.Directory`     | Local directory for storing archives              |
| `Archives.Local.Origin`        | Public server origin (`https://example.com`)      |
| `Archives.Local.UrlPrefix`     | URL path prefix for download URLs                 |
| `Archives.S3.AccessKey`        | S3 access key ID                                  |
| `Archives.S3.SecretKey`        | S3 access key secret                              |
| `Archives.S3.Region`           | AWS region identifier                             |
| `Archives.S3.BucketName`       | S3 bucket name                                    |
| `Archives.S3.StorageClass`     | S3 storage class for archives                     |
| `Archives.S3.ArchiveDirectory` | S3 subdirectory for uploading archives            |
| `Authentication.AllowSignUp`   | Whether to allow registration using the dashboard |
| `ConnectionStrings`            | Database connection strings                       |
| `Server.BaseUrl`               | Application public URL                            |
| `Provider`                     | Database provider (`postgres`, `sqlite`)          |

The following table displays list of first class configuration options supported by Courier server. If you provide configs using json file, the dot on config keys represents child objects.

```json
{
  "Authentication": {
    "AllowSignUp": false
  }
}
```

For passing configurations using environment variables replace dots on config keys with double underline (`__`). The above configuration could be passed with the following environment variable.

```sh
export Authentication__AllowSignUp=false
```

### Database

Courier server currently supports using SQLite or PostgreSQL server as data source. To connect to a database you have to configure 2 config parameters: `Provider`, `ConnectionStrings`.

Provider value could be either `"postgres"` or `"sqlite"` respectively corresponding to PostgreSQL and SQLite databases. Connection strings used to represent database connection details to database drivers. Each database provider accepts different set of configurations. Parameters are represented as a string, each key value delimitered by equal (`=`) joined with semicolon (`;`). 

**SQLite** - `ConnectionStrings.SqliteConnection`

SQLite connection strings does look like the following. 

```
Data Source=/path/to/data.db;Cache=Shared
```

- [SQLite Connection strings](https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/connection-strings)

**PostgreSQL** - `ConnectionStrings.PostgresConnection`

Example PostgreSQL connection string looks like that:

```
Host=127.0.0.1;Username=user;Password=pass;Database=mydb
```

- [Npgsql Connection string parameters](https://www.npgsql.org/doc/connection-string-parameters.html)

### Storage

Courier server currently supports limited set of storage backends which includes: `local` and `s3` - that could be configured using `Archives.Type` config.

Local storage backend stores archives on disk and serves using them using courier server. To use local storage backend you have to set `Archives.Local.Directory` and `Archives.Local.Origin` configs. On containerized environments don't forget to mount volume from host machine and use mounted directory for storing archives, otherwise restarting container will result in data loss.

If you want to store content on cloud, you can use S3 storage backend by configuring the following config keys: `Archives.S3.AccessKey`, `Archives.S3.SecretKey`, `Archives.S3.Region`, `Archives.S3.BucketName`.

> Note: Currently only S3 buckets storeed on AWS is supported. Migrating to cloud provider independent S3 storage manager is planned to be developed.

