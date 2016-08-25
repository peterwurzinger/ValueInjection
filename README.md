# ValueInjection

*ValueInjection* can provide a solution for a special requirement mostly met in enterprise environments. I see it more as a pattern than a library, so that's the reason I didn't provide a NuGet-Package for it yet.

## The Problem

So this is the part where it gets a little esoteric.
Imagine your Domain Model representing your Business Requirements. Now there may be the case that you have to reference (and further present) centralized data, which are unlikely to change often. Think of a national address register managing all existing addresses in your country, for example.
A requirement in your application could be, that you need to save and validate the user's address for shipping purposes.

To sum it up: You got your domain model, you got centralized (static) data to which you maybe only have read-access to and you got a relation between them. Keep in mind, that persisting read-once external Datasets can potentially lead to massive synchronization issues. To stress the example further: Street names change every now and then. If you did something like 
```
var adress = ReadAdressFromExternalSource(...);
person.adress.street = adress.street;
person.adress...
```

you have to keep your instances of external data in sync with it's origin.

## The theoretical Solution

So the basic idea is to keep going with relational modeling, treat the problem as what it is - a relationship - and let your Domain Objects reference the external Domain Objects through saving their key. So change
```
Person {FirstName, LastName, AddressKey, AddressCity, AddressStreet, ...}
```
to
```
Person  {FirstName, LastName, AddressKey -> external[Address.Key]}
```

This is relational modeling, so nothing new for now. The artifact *external[]* is of course not supported in a relational model, it's morelike *ValueInjection*s time to shine.

***Note:*** As you can see, this pattern only applies to external entities with an unchangable key, like tuples in relational databases and anything else providing entities with unique property-values. If your external datasource doesn't provide such a key, you're doomed anyway - have nice day!

## Man, you bore me

You might have noticed, that the theoretical-solution-stuff didn't solve any problem at all, but it's needed to fully understand the pattern. It will get more juicy, I promise!

Now we're at the point where our Business Model is in the third normal form regarding external data, but now we ended up building constructs like
```
var query = Entities<Person>()...ToList().ForEach(p =>
		{
			p.AddressCity   = GetAddressCityFromExternalSource(p.AddressKey);
			p.AddressStreet = GetAddressStreetFromExternalSource(p.AddressKey);
		});
```

For one or two queries in a static application this is fine - stop reading and do that!
But if you expect your enterprise application to get larger you might get annoyed by post processing your Domain Objects all the time.

***TODO***