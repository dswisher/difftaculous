#![Difftaculous](doc/difftaculous.png)

- [License](LICENSE.md)

# Summary #

Difftaculous is a .Net library that can be used to find the differences
between two structured objects.  The objects can be .Net objects, JSON,
or XML.

Not only does it return a boolean flag indicating whether the objects
differ, it also returns details about the differences themselves.

Consider the following two JSON files:

    {                                     {
    "father": {                           "father": {
        "firstName": "Fred",                  "firstName": "Fred",
        "lastName": "Flintstone",             "lastName": "Flintstone",
        "age": 28                             "age": 28
    },                                    },
    "mother": {                           "mother": {
        "firstName": "Wilma",                 "firstName": "Wilma",
        "lastName": "Flintstone",             "lastName": "Flintstone",
        "age": 26                             "age": 27
    },                                    },
    "firstName": "Bam-Bam"                "firstName": "BamBam"
	}                                     }

Here is a snippet of code that uses Difftaculous to compare them:

	string a = LoadFile("SimpleJsonA.json");
	string b = LoadFile("SimpleJsonB.json");
	IDiffResult result = DiffEngine.Compare(new JsonAdapter(a),
                                            new JsonAdapter(b));
	Console.WriteLine("AreSame: {0}", result.AreSame);
	foreach (var anno in result.Annotations)
	{
    	Console.WriteLine("{0}: {1}", anno.Path, anno.Message);
	}

When run, this code outputs the following:

    AreSame: False
	mother.age: values differ: '26' vs. '27'
	firstName: values differ: 'Bam-Bam' vs. 'BamBam'


# Motivation #

This project was born out of a need to compare the output of two web
services to ascertain if they return similar results.  The web services
return time series data, which motivated the hinting mechanism.

# Status #

The project is still in its infancy, but it is already at the point where
it is useful.  The public API is relatively stable, but it may yet change
as new features are added.  At this point, the library should still be
considered alpha.

Comments and feedback are most welcome.

