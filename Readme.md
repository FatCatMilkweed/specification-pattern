Specification Design Pattern implementation example in C#
You can read more about specification pattern [here](https://en.wikipedia.org/wiki/Specification_pattern).

Preparing custom specifications:
```CSharp
public static class ServiceTypeSpecifications
{
    public static Specification<ServiceType> IsAvailable()
    {
        return new Specification<ServiceType>(x => x.IsAvailable);
    }
    
    public static Specification<ServiceType> IsFree()
    {
        return new Specification<ServiceType>(x => x.IsFree);
    }
}
```

Usage example with EF Core:
```CSharp
var availableServiceTypes = _dbContext.ServiceTypes
    .Where(ServiceTypeSpecifications.IsAvailable())
    .ToList();
```

Or you can combine specifications using && or ||:
```CSharp
var availableServiceTypes = _dbContext.ServiceTypes
    .Where(ServiceTypeSpecifications.IsAvailable() && ServiceTypeSpecifications.IsFree())
    .ToList();
```