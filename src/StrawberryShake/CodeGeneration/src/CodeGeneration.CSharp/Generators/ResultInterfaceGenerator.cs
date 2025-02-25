using System.Linq;
using StrawberryShake.CodeGeneration.CSharp.Builders;
using StrawberryShake.CodeGeneration.CSharp.Extensions;
using StrawberryShake.CodeGeneration.Descriptors.TypeDescriptors;
using static StrawberryShake.CodeGeneration.Utilities.NameUtils;

namespace StrawberryShake.CodeGeneration.CSharp.Generators;

public class ResultInterfaceGenerator : CodeGenerator<InterfaceTypeDescriptor>
{
    protected override void Generate(InterfaceTypeDescriptor descriptor,
        CSharpSyntaxGeneratorSettings settings,
        CodeWriter writer,
        out string fileName,
        out string? path,
        out string ns)
    {
        fileName = descriptor.RuntimeType.Name;
        path = null;
        ns = descriptor.RuntimeType.NamespaceWithoutGlobal;

        InterfaceBuilder interfaceBuilder = InterfaceBuilder
            .New()
            .SetComment(descriptor.Description)
            .SetName(fileName);

        foreach (PropertyDescriptor prop in descriptor.Properties)
        {
            interfaceBuilder
                .AddProperty(prop.Name)
                .SetComment(prop.Description)
                .SetType(prop.Type.ToTypeReference())
                .SetPublic();
        }

        interfaceBuilder.AddImplementsRange(descriptor.Implements.Select(x => x.Value));

        foreach (DeferredFragmentDescriptor deferred in descriptor.Deferred)
        {
            var propertyName = GetPropertyName(deferred.Label);

            // Add fragment property
            interfaceBuilder
                .AddProperty(propertyName)
                .SetType($"{deferred.InterfaceName}?")
                .SetPublic();
        }

        interfaceBuilder.Build(writer);
    }
}
