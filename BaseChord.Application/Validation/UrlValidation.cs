using System.Text.RegularExpressions;
using FluentValidation;

namespace BaseChord.Application.Validation;

/// <summary>
/// Contains a fluentvalidation rule for url 
/// </summary>
public static class UrlValidation
{
    private const string UrlRegex = @"(https?:\/\/)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)";

    private const string IpAddressRegex =
        @"((^\s*((([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]))\s*$)|(^\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:)))(%.+)?\s*$))";

    /// <summary>
    /// Adds a Rule that prevents the string to contain a URL or an IP-Address
    /// <see cref="NoUrl{T}"/>
    /// <see cref="NoIpAddress{T}"/>
    /// </summary>
    /// <param name="ruleBuilderInitial">The fluentvalidation rule builder <see cref="IRuleBuilderInitial{T,TProperty}"/></param>
    /// <typeparam name="T"></typeparam>
    /// <returns><see cref="IRuleBuilderInitial{T,TProperty}"/></returns>
    public static IRuleBuilder<T, string?> NotServer<T>(this IRuleBuilder<T, string?> ruleBuilderInitial)
    {
        ruleBuilderInitial.NoUrl();
        ruleBuilderInitial.NoIpAddress();

        return ruleBuilderInitial;
    }
    
    /// <summary>
    /// Adds a Rule that prevents the string to contain a URL
    /// </summary>
    /// <param name="ruleBuilderInitial">The fluentvalidation rule builder <see cref="IRuleBuilderInitial{T,TProperty}"/></param>
    /// <typeparam name="T"></typeparam>
    /// <returns><see cref="IRuleBuilderInitial{T,TProperty}"/></returns>
    public static IRuleBuilder<T, string?> NoUrl<T>(this IRuleBuilder<T, string?> ruleBuilderInitial)
    {
        ruleBuilderInitial
            .Must(x => !Regex.IsMatch(x ?? string.Empty, UrlRegex))
            .WithMessage("The Value provided contains an URL");

        return ruleBuilderInitial;
    }
    
    /// <summary>
    /// Adds a Rule that prevents the string to contain an ip-address
    /// </summary>
    /// <param name="ruleBuilderInitial">The fluentvalidation rule builder <see cref="IRuleBuilderInitial{T,TProperty}"/></param>
    /// <typeparam name="T"></typeparam>
    /// <returns><see cref="IRuleBuilderInitial{T,TProperty}"/></returns>
    public static IRuleBuilder<T, string?> NoIpAddress<T>(this IRuleBuilder<T, string?> ruleBuilderInitial)
    {
        ruleBuilderInitial
            .Must(x => !Regex.IsMatch(x ?? string.Empty, IpAddressRegex))
            .WithMessage("The Value provided contains an IP Address");

        return ruleBuilderInitial;
    }
}