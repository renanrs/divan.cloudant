# divan.cloudant
[![Build status](https://ci.appveyor.com/api/projects/status/b6w3aafn14l8m5ut/branch/master?svg=true)](https://ci.appveyor.com/project/renanrs/divan-cloudant-scv9l/branch/master)

Framework to simplify the use of Cloudant Database. It provides a friendly interface to communicate to Cloudant's API.

## Getting Started

Divan is a nuget package and you must only install through nuget where will provide the stable version.

### Prerequisites

It supports only .netcore projects.

### Adding Divan.Cloudant to your project

#### Package Manager  
```
PM> Install-Package Divan.Cloudant
```

#### .NET CLI

```
> dotnet add package Divan.Cloudant
```

## Code Sample
The first step to use Divan.Cloudant, you need to create an instance of `CloudantClient`, this class makes possible database handling, such as Create, Get and Delete.

```c#
var cloudantClient = new CloudantClient(new CloudantConnection("CLOUDANT_URL","CLOUDANT_USER","CLOUDANT_PWD"));
```

### Creating Database
Cloudant API allows for easy development creation of databases. Inside the `CloudantClient`, exists the method `CreateDbAsync` or `GetDbAsync` to get the work done.

`CreateDbAsync` only creates the database in Cloudant.
```c#
var task = await cloudantClient.CreateDbAsync("myDatabase");
```
`GetDbAsync` different of `CreateDbAsync`, if there's no database of the same name it creates a new one and returns an instance of 'Database'.
```c#
var database = await cloudantClient.GetDatabaseAsync("myDatabase");
```
### Creating Documents

Divan.Cloudant serializes your model to json internally and send to Cloudant.
By default Cloundant's documents has two required properties, _id and _rev, their values is generated automatically. Divan.Cloudant requires those properties in your models to be serialized to Json format and vice versa.

Model
```c#
public class ZipCodeModel    {

        public string _id { get; set; }
        public string _rev { get; set; }
        public string ZipCode { get; set; }
        public string Address { get; set; }
        public string Neighborhood { get; set; }
}
```

`CreateDocAsync`

```c#
var db = await cloudantClient.GetDatabaseAsync("myDatabase");
var doc = new ZipCodeModel
            {
                _id = null,
                _rev = null,
                ZipCode = "18103610",
                Neighborhood = "Jardim Primavera",
                Address = "Rua AngeloZanardo"
            };
ResultObject resultObject = await db.CreateAsync<ZipCodeModel>(doc);
Console.WriteLine($"Document id is: {resultObject.id}");
```

## Authors

* **Renan Silveira** - *Initial work* - [Divan.Cloudant](https://github.com/renanrs/divan.cloudant)

See also the list of [contributors](https://github.com/renanrs/divan.cloudant/contributors) who participated in this project.

## License

This project is licensed under the GNU License - see the [LICENSE.md](LICENSE.md) file for details


