# Discriminated Unions T4 generator


## Basic usage

#### 1. Install T4DU package from NuGet. 
Package is available here: https://www.nuget.org/packages/T4DU. Do not forget to check "Include prerelease" checkbox for beta version.

This will add UnionsGen.tt to your project. This is a regular T4 file ([Official T4 Docs](https://docs.microsoft.com/ru-ru/visualstudio/modeling/code-generation-and-t4-text-templates)). Also your project will get a reference to assembly containing [UnionBase] attribute.
#### 2. Write your discriminated unions:
```cs
[UnionBase]
public abstract partial class Shape { }

public partial class Rectangle : Shape
{
    public double Width { get; }
    public double Height { get; }
}

public partial class Circle : Shape
{
    public double Radius { get; }
}
```



#### 3. Run code generation 
Either open and save .tt file or use Build -> Transform All T4 Templates.

#### 4. That's it, now you are ready to use discriminated unions:
 
```cs
Shape shape = Shape.Rectangle(width: 10,  height: 2);
double area = shape.Match(rectangle: r => r.Height * r.Width,
                          circle:    c => Math.PI * c.Radius * c.Radius);
Console.WriteLine(area); // 20
```

## What is generated for you

1. Match method that accepts Func<Case, TResult> for each case in Discriminated Union and returns TResult.
2. Do method that accepts Action<Case> for each case in Discriminated Union and returns nothing.
3. Record type semantics: constructor, static constructors available from base class (e.g. Shape.Rectange(...)), Equals, GetHashCode() and ==,!= operators.

## Notes
By default T4DU assumes that your project directory and packages directory are in the same solution directory. If this is not the case you should manually modify UnionsGen.tt to reference .ttinclude file from appropriate packages folder.

Base class must have [UnionBase] attribute. It is used by T4DU to find it. Cases must be located in the same assembly and derive from base class.

Make sure that base class is marked as abstract. Also base class and case classes must have partial keyword, so code generator can extend their definitions.

Note that you don't have to create constructors, T4DU generates that for you (including static ones like Shape.Rectangle(width, height)! Also you get Equals, GetHashCode and ==, != operators for free.

To avoid extension of the hierarchy from other assemblies, base class has **internal abstract void Seal()** method. This method is implemented by generated case classes (with noop body) but cannot be implemented by other assemblies since it is internal. Also case classes are marked with **sealed** keyword for similar reasons.

While T4DU uses Roslyn which requires .NET 4.6, T4DU doesn't add any .NET 4.6 assembly references to your code. This means that you can generate discriminated unions in a .NET 3.5+ project.
