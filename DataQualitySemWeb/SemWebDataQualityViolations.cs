using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualitySemWebLib
{
    public enum SemWebDataQualityViolations
    {
        None,
        MultiplePropertiesDefined,
        MultipleValuesFound,
        DuplicateValuesFound,
        MixedNodeTypesFound,
        PropertyNameCloselyMatchesAnotherProperty,
        ValueWasPreviouslySpecifiedAsURI,
        ValueWasPreviouslySpecifiedAsLiteral,
        UnableToDereferenceURI,
        UnableToDereferenceURIFoundSearchedAlternative,
        UnableToResolveValueHyperlink,
        MultipleLabelValuesFound
    }

    public static class SemWebDataQualityViolationProcessor
    {
        public static void Log(IViolationMessageStore ivms, SemWebDataQualityViolations violation)
        {

            switch(violation)
            {
                case SemWebDataQualityViolations.DuplicateValuesFound:
                    ivms.AddViolationMessage( "Duplicate values were found for this entry." );
                    break;
                case SemWebDataQualityViolations.MixedNodeTypesFound:
                    ivms.AddViolationMessage("Node has both literal values and also URI nodes defined.");
                    break;
                case SemWebDataQualityViolations.MultiplePropertiesDefined:
                    ivms.AddViolationMessage("Multiple properties have been defined for this entry.");
                    break;
                case SemWebDataQualityViolations.MultipleValuesFound:
                    ivms.AddViolationMessage("Mulitple values have been found for this entry.");
                    break;
                case SemWebDataQualityViolations.PropertyNameCloselyMatchesAnotherProperty:
                    ivms.AddViolationMessage("Property name closely matches another property.");
                    break;
                case SemWebDataQualityViolations.UnableToDereferenceURI:
                    ivms.AddViolationMessage("Could not deference URI.");
                    break;
                case SemWebDataQualityViolations.UnableToResolveValueHyperlink:
                    ivms.AddViolationMessage("Node value is referring to hyperlink that is not available.");
                    break;
                case SemWebDataQualityViolations.ValueWasPreviouslySpecifiedAsLiteral:
                    ivms.AddViolationMessage("The value for this entry has previously been defined as a literal value but is now a URI.");
                    break;
                case SemWebDataQualityViolations.ValueWasPreviouslySpecifiedAsURI:
                    ivms.AddViolationMessage("This value for this entry is currently defined as a literal but was previously defined as a URI.");
                    break;
                case SemWebDataQualityViolations.MultipleLabelValuesFound:
                    ivms.AddViolationMessage("Multiple label properties found.");
                    break;
                case SemWebDataQualityViolations.UnableToDereferenceURIFoundSearchedAlternative:
                    ivms.AddViolationMessage("Unable To DereferenceURI, however, searched alternative was found.");
                    break;
            }   
        }
    }
}
