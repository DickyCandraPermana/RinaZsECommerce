using System;

namespace RinaZsECommerce.Domain.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class NotAuditableAttribute : Attribute
{

}