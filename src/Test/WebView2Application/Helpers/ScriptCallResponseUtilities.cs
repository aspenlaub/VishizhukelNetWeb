using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.VishizhukelNetWeb.Test.WebView2Application.Helpers;

public static class ScriptCallResponseUtilities {
    public static IErrorsAndInfos ToTestRunErrorsAndInfos(ScriptCallResponse scriptCallResponse) {
        var errorsAndInfos = new ErrorsAndInfos();
        if (scriptCallResponse == null) {
            errorsAndInfos.Errors.Add(Properties.Resources.NoResultReceived);
            return errorsAndInfos;
        }

        if (scriptCallResponse.Success.Inconclusive) {
            errorsAndInfos.Errors.Add(Properties.Resources.ResultIsInconclusive);
        }

        if (!string.IsNullOrEmpty(scriptCallResponse.ErrorMessage)) {
            errorsAndInfos.Errors.Add(scriptCallResponse.ErrorMessage);
        }
        if (scriptCallResponse.Success.Inconclusive) {
            return errorsAndInfos;
        }

        switch (scriptCallResponse.Success.YesNo) {
            case false when !errorsAndInfos.Errors.Any():
                errorsAndInfos.Errors.Add(Properties.Resources.TestCaseFailed);
                break;
            case true when !errorsAndInfos.Infos.Any():
                errorsAndInfos.Infos.Add(Properties.Resources.TestCaseSucceeded);
                break;
        }

        return errorsAndInfos;
    }

    public static ScriptCallResponse Invert(ScriptCallResponse scriptCallResponse, string expectedErrorMessage) {
        if (scriptCallResponse.Success.Inconclusive) {
            return scriptCallResponse;
        }

        switch (scriptCallResponse.Success.YesNo) {
            case true:
                return new ScriptCallResponse {
                    Success = new YesNoInconclusive { Inconclusive = false, YesNo = false },
                    ErrorMessage = string.Format(Properties.Resources.ScriptCallFailureExpected, expectedErrorMessage)
                };
            case false when !scriptCallResponse.ErrorMessage.Contains(expectedErrorMessage, StringComparison.InvariantCultureIgnoreCase):
                scriptCallResponse.Success = new YesNoInconclusive { Inconclusive = false, YesNo = false };
                scriptCallResponse.ErrorMessage = string.Format(Properties.Resources.ScriptCallFailedButWrongErrorMessage, expectedErrorMessage);
                return scriptCallResponse;
            default:
                return new ScriptCallResponse {
                    Success = new YesNoInconclusive { Inconclusive = false, YesNo = true }
                };
        }
    }

    public static ScriptCallResponse VerifyExpectedClasses(ScriptCallResponse scriptCallResponse, IEnumerable<string> expectedClasses, string expectedNthOfWhichClass, int n) {
        if (scriptCallResponse?.Success == null) {
            return new ScriptCallResponse {
                Success = new YesNoInconclusive { Inconclusive = false, YesNo = false },
                ErrorMessage = Properties.Resources.ScriptCallFailedTechnically
            };
        }

        if (scriptCallResponse.Success.Inconclusive || !scriptCallResponse.Success.YesNo) {
            return scriptCallResponse;
        }

        if (scriptCallResponse.DomElement?.Classes == null) {
            return new ScriptCallResponse {
                Success = new YesNoInconclusive { Inconclusive = false, YesNo = false },
                ErrorMessage = Properties.Resources.ScriptCallSucceededButNoDomElement
            };
        }

        string expectedClass = expectedClasses.FirstOrDefault(c => !scriptCallResponse.DomElement.Classes.Contains(c));
        if (expectedClass != null) {
            return new ScriptCallResponse {
                Success = new YesNoInconclusive { Inconclusive = false, YesNo = false },
                ErrorMessage = string.Format(Properties.Resources.ScriptCallSucceededButDomElementDoesNotHaveClass, expectedClass)
            };
        }

        if (expectedNthOfWhichClass == "") {
            return scriptCallResponse;
        }

        if (scriptCallResponse.DomElement?.NthOfClass == null) {
            return new ScriptCallResponse {
                Success = new YesNoInconclusive { Inconclusive = false, YesNo = false },
                ErrorMessage = Properties.Resources.ScriptCallSucceededButNoNthOfClass
            };
        }

        if (scriptCallResponse.DomElement?.NthOfClass.Class != expectedNthOfWhichClass || scriptCallResponse.DomElement?.NthOfClass.N != n) {
            return new ScriptCallResponse {
                Success = new YesNoInconclusive { Inconclusive = false, YesNo = false },
                ErrorMessage = Properties.Resources.ScriptCallSucceededButMisMatchInNthOfClass
            };
        }

        return scriptCallResponse;
    }
}