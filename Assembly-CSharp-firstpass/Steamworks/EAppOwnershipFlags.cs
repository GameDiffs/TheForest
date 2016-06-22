using System;

namespace Steamworks
{
	[Flags]
	public enum EAppOwnershipFlags
	{
		k_EAppOwnershipFlags_None = 0,
		k_EAppOwnershipFlags_OwnsLicense = 1,
		k_EAppOwnershipFlags_FreeLicense = 2,
		k_EAppOwnershipFlags_RegionRestricted = 4,
		k_EAppOwnershipFlags_LowViolence = 8,
		k_EAppOwnershipFlags_InvalidPlatform = 16,
		k_EAppOwnershipFlags_SharedLicense = 32,
		k_EAppOwnershipFlags_FreeWeekend = 64,
		k_EAppOwnershipFlags_RetailLicense = 128,
		k_EAppOwnershipFlags_LicenseLocked = 256,
		k_EAppOwnershipFlags_LicensePending = 512,
		k_EAppOwnershipFlags_LicenseExpired = 1024,
		k_EAppOwnershipFlags_LicensePermanent = 2048,
		k_EAppOwnershipFlags_LicenseRecurring = 4096,
		k_EAppOwnershipFlags_LicenseCanceled = 8192,
		k_EAppOwnershipFlags_AutoGrant = 16384
	}
}
