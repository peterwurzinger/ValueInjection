# ValueInjection

*ValueInjection* can provide a solution for a special requirement mostly met in enterprise environments. I see it 
more as a pattern than a library, so that's the reason I didn't provide a NuGet-Package for it yet.

## The Problem

So this is the part where it gets a little esoteric.
Imagine your Domain Model representing your Business Requirements. There may be the case that you have to reference 
(and further present) centralized data, which are unlikely to change often or you simply do not want to persist in 
your Domain Model. At least one of your Domain Objects has a relation to a Domain Object from another 
domain - for simplicity those objects are further called "external (Domain/Business) Objects".

A requirement in your application could be, that you need to save and validate the user's address for shipping 
purposes. Therefore you bought read-acces to a national address register managing all existing addresses in your 
country.

The simplest possible solution would be saving the Address-Data when the user enters it.
```
Person person = new Person();

var adress = ReadAddressFromExternalSource(<user input>);
person.AdressKey = adress.Key;
person.AdressCity = adress.City;
person.AdressStreet = adress.Street;
person.Adress...

Entities<Person>.Save(person);
```

For static, never ever changing data this is fine. If the data can **potentially** change, you encounter 
the problem to keep your instances of external data in sync with it's origin.

## The theoretical Solution

So the basic idea is to keep going with relational modeling, treat the problem as what it is - a relationship - 
and let your Domain Objects reference the external Domain Objects through saving their key. So change
```
Person {FirstName, LastName, AddressKey, AddressCity, AddressStreet, ...}
```
to
```
Person  {FirstName, LastName, AddressKey -> external[Address.Key]}
```

This is relational modeling, so nothing new for now. The artifact *external[]* is of course not supported in a 
relational model, it's morelike *ValueInjection*s time to shine.

***Note:*** As you can see, this pattern only applies to external entities with an unchangable key, like tuples 
in relational databases and anything else providing entities with unique property-values. If your external 
datasource doesn't provide such a key, you're doomed anyway - have nice day!

## Man, you bore me

You might have noticed, that the theoretical-solution-stuff didn't solve any problem at all, but it's needed to 
fully understand the pattern. It will get more juicy, I promise!

Now we're at the point where our Business Model is in the third normal form regarding external data, but mostly you  
end up building constructs like
```
var query = Entities<Person>().Select(p => new PersonView {...}).ToList().ForEach(p =>
		{
			p.AddressCity   = GetAddressCityFromExternalSource(p.AddressKey);
			p.AddressStreet = GetAddressStreetFromExternalSource(p.AddressKey);
		});
```

For one or two queries in a static application this is fine - stop reading and do that!
But if you expect your application to get larger you might get annoyed by post processing your Domain Objects/Views 
all the time.

*ValueInjection*s approach is implementing a service to provide ```Address``` - objects by its key and inject the 
appropriate values into your DOs/Views via a custom Enumerator. Let's change our example above the way the pattern 
allows it to get.

```
class AddressObtainer : IValueObtainer<Address> {
        public Address ObtainValue(object key)
        {
            return CallAddressResolutionService((Guid)key);
        }

        object IValueObtainer.ObtainValue(object key)
        {
            return ObtainValue(key);
        }
}
```
In your application bootstrapper (like Global.asax, Startup.cs,...) do
```
ValueInjector.UseValueObtainer(new AddressObtainer());
```

Annotate your ```PersonView``` like this
```
    ...

    public Guid AddressKey { get; set; }

    [ValueInjection(typeof(Address), nameof(AddressKey), nameof(Address.City))]
    public string AddressCity { get;set; }

    [ValueInjection(typeof(Address), nameof(AddressKey), nameof(Address.Street))]
    public string AddressStreet { get; set; }
```

When querying your to PersonViews mapped Persons do
```
var query = Entities<Person>().Select(p => new PersonView {...}).ToValueInjection()
```

And that's it. Enumerating this query will trigger *ValueInjection* and both properties will be filled with 
external data.

# TODO

As you can see the documentation is still work in progress. There are several built-in features I want to cover 
but sadly did not have the time to document appropriately.
Im talking about
- Caching once obtained values
- Caching analyzed metadata
- Static configuration via fluent-API
- Configuration done for Interfaces/Base Types

# Closing Words
I guess it's obvious, that English is not my mother tongue. If you find typos in the code or the documentation 
please feel free to contact me or fix it on your own. Also if you think you can provide an enhancement 
for the library/pattern I would highly appreciate pull requests or suggestions.

So that's it for now. Thanks for reading and if I was able to help you with an encountered problem I would be 
happy to hear from you.

Cheers