using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

/* 
 * File: \Libraries\LibUnitConversions.  This library provides various methods related to converting numeric values.  This file will need to be 
 * copied into project or added via 'Add as Link'.  This file will not compile into a Windows Runtime Component library that can be used by adding 
 * a Reference since it uses Decimal types.  At his time, Decimals do not have a supported/equivalent underlying Windows Runtime type.
 * 
 * Normally the namespace below would be proceeded by the project name but is ommitted so files can be shared between projects easily.
 * Common namespace 'LibraryCoder' is used instead.
 * 
 * Handy online calculator by Casio that allows math to extreme digits.  http://keisan.casio.com/calculator
 * Tested many values below that use decimal division and/or mutiplication versus doing math upfront to 34 digits using this online calculator.
 * In all cases tested, decimal math below returns exact same result as this calculator carried out to 34 digits.  Bottom line, no precision is
 * lost by doing conversion calculations in the various list below versus doing the calculation elsewhere and importing the result.  Calculations
 * in the various list below using mutiple variable is much more clearer versus just a bunch of random digits and also much easier to propagete
 * to new conversions.
 * 
 * Metric prefixes: https://en.wikipedia.org/wiki/Metric_prefix
 * 
 * NIST Reference on Constants, Units, and Uncertainty: http://physics.nist.gov/cuu/index.html
 * 
 * In 1958, a conference of English-speaking nations agreed to unify their standards of length and mass to define them in terms 
 * of metric measures. As result, American yard was shortened and Imperial yard was lengthened. The new conversion factors were 
 * announced in 1959 in Federal Register Notice 59-5442 (June 30, 1959).  It states an inch is exactly equal to 2.54 cm.
 * Web Site: http://www.nist.gov/pml/wmd/metric/length.cfm
 * 
 * Conversion of Units: This Wiki site has it all, more or less!  https://en.wikipedia.org/wiki/Conversion_of_units
 * 
 * Metric System:  https://en.wikipedia.org/wiki/Metric_system
 * International System of Units (SI):   https://en.wikipedia.org/wiki/International_System_of_Units
 * United States customary units (USC):  https://en.wikipedia.org/wiki/United_States_customary_units
 * Imperial units or British Imperial:   https://en.wikipedia.org/wiki/Imperial_units
 *
 * Exception Handling: Following methods check for OverflowException. If OverflowException occurs, then methods throw exception back
 * to calling method to be handled there.  Calling methods should check for exception via try-catch blocks. For additional details refer
 * to LibUC.TestMethodsMain().
 */

namespace LibraryCoder.UnitConversions
{
    /// <summary>
    /// Class used for storing values of a single conversion.  This class is identical to class LibUCBase but is read/write versus readonly.
    /// Use this class in calling Apps to allow updates/changes if EnumConversionsType changes.
    /// </summary>
    public class LibUCBaseRW     // Shorthand for UnitConversionsBaseReadWrite.
    {
        // Class is read/write since contains { get; set; }
        /// <summary>
        /// Conversion unit Enum. Sample: EnumConversionsLength.inch. Use Enum base class since enum value changes with each EnumConversionsType.
        /// </summary>
        public Enum EnumConversions { get; set; }
        /// <summary>
        /// Conversion unit description. Sample: "inch".
        /// </summary>
        public string StringDescription { get; set; }
        /// <summary>
        /// Conversion unit symbol. Sample" "in".
        /// </summary>
        public string StringSymbol { get; set; }
        /// <summary>
        /// Conversion unit value relative to base unit value. Sample: 0.0254m (1 inch = 0.0254 meter exact. Meter is base unit and is 1.0m).
        /// </summary>
        public decimal DecimalBase { get; set; }
        /// <summary>
        /// Conversion unit website link that provides information about unit. Default is Empty String. Sample: "https://en.wikipedia.org/wiki/Inch"
        /// </summary>
        public string StringHyperlink { get; set; }

        /// <summary>
        /// Constructor must have same name as it's class.
        /// </summary>
        /// <param name="_EnumConversions">Conversion unit Enum. Sample: EnumConversionsLength.inch. Use Enum base class since enum value changes with each EnumConversionsType.</param>
        /// <param name="_StringDescription">Conversion unit description. Sample: "inch".</param>
        /// <param name="_StringSymbol">Conversion unit symbol. Sample: Symbol for "inch" is "in".</param>
        /// <param name="_DecimalBase">Conversion unit value relative to base unit value. Sample: 0.0254m (1 inch = 0.0254 meter exact. Meter is base unit and is 1.0m).</param>
        /// <param name="_StringHyperlink">Conversion unit website link that provides information about unit. Default is Empty String. Sample: "https://en.wikipedia.org/wiki/Inch"</param>
        public LibUCBaseRW(Enum _EnumConversions, string _StringDescription, string _StringSymbol, decimal _DecimalBase, string _StringHyperlink)
        {
            EnumConversions = _EnumConversions;
            StringDescription = _StringDescription;
            StringSymbol = _StringSymbol;
            DecimalBase = _DecimalBase;
            StringHyperlink = _StringHyperlink;
        }

        public override string ToString()
        {
            return $"EnumConversions={EnumConversions}, StringConversionsType={StringDescription}, StringSymbol={StringSymbol}, DecimalBase={DecimalBase}, StringConversionsTypeLink={StringHyperlink}";
        }
    }

    /// <summary>
    /// Class used for storing values of a single conversion. This class is identical to class LibUCBaseRW but is readonly versus read/write.
    /// Use this class to build various lists containing mutiple conversions in each list since list values never change.
    /// </summary>
    public class LibUCBase     // Shorthand for LibraryUnitConversionsBase.
    {
        // Class is readonly since only contains { get; }.
        /// <summary>
        /// Conversion unit Enum. Sample: EnumConversionsLength.inch. Use Enum base class since enum value changes with each EnumConversionsType.
        /// </summary>
        public Enum EnumConversions { get; }
        /// <summary>
        /// Conversion unit description. Sample: "inch".
        /// </summary>
        public string StringDescription { get; }
        /// <summary>
        /// Conversion unit symbol. Sample" "in".
        /// </summary>
        public string StringSymbol { get; }
        /// <summary>
        /// Conversion unit value relative to base unit value. Sample: 0.0254m (1 inch = 0.0254 meter exact. Meter is base unit and is 1.0m).
        /// </summary>
        public decimal DecimalBase { get; }
        /// <summary>
        /// Conversion unit website link that provides information about unit. Default is Empty String. Sample: "https://en.wikipedia.org/wiki/Inch"
        /// </summary>
        public string StringHyperlink { get; }
 
        /// <summary>
        /// Constructor must have same name as it's class.
        /// </summary>
        /// <param name="_EnumConversions">Conversion unit Enum. Sample: EnumConversionsLength.inch. Use Enum base class since enum value changes with each EnumConversionsType.</param>
        /// <param name="_StringDescription">Conversion unit description. Sample: "inch".</param>
        /// <param name="_StringSymbol">Conversion unit symbol. Sample: Symbol for "inch" is "in".</param>
        /// <param name="_DecimalBase">Conversion unit value relative to base unit value. Sample: 0.0254m (1 inch = 0.0254 meter exact. Meter is base unit and is 1.0m).</param>
        /// <param name="_StringHyperlink">Conversion unit website link that provides information about unit. Default is Empty String. Sample: "https://en.wikipedia.org/wiki/Inch"</param>
        public LibUCBase(Enum _EnumConversions, string _StringDescription, string _StringSymbol, decimal _DecimalBase, string _StringHyperlink)
        {
            EnumConversions = _EnumConversions;
            StringDescription = _StringDescription;
            StringSymbol = _StringSymbol;
            DecimalBase = _DecimalBase;
            StringHyperlink = _StringHyperlink;
        }

        public override string ToString()
        {
            return $"EnumConversions={EnumConversions}, StringConversionsType={StringDescription}, StringSymbol={StringSymbol}, DecimalBase={DecimalBase}, StringConversionsTypeLink={StringHyperlink}";
        }
    }

    /* Beginning of Conversion Enumerations ************************************************************************************/

    // Sort following enumeration alphabetically.  Names of various conversion enumerations below must match a value in this enumeration.
    // Below method GetConversionValues() contains switch statement that needs to be updated with any new additions to this enumeration.
    /// <summary>
    /// Enumeration of available conversion types.
    /// </summary>
    public enum EnumConversionsType
    {
        Acceleration, Angle, Area, [Display(Description = "Area Moment First")] AreaMomentFirst, [Display(Description = "Area Moment Second")] AreaMomentSecond, 
        Data, Density, Energy, Flow, Force, Frequency, [Display(Description = "Length/Distance")] Length, Mass, Metric, Power, Pressure, Speed, 
        [Display(Description = "Solid Angle")] SolidAngle, Temperature, Time, Torque, Volume
    };
   
    // Note: Must set base unit as first item in all conversion enumerations below so it can be found when needed.

    /// <summary>
    /// Enumeration of available Acceleration conversions.
    /// </summary>
    public enum EnumConversionsAcceleration
    {
        meter_per_sec_squared, decimeter_per_sec_squared, centimeter_per_sec_squared, millimeter_per_sec_squared, decameter_per_sec_squared,
        hectometer_per_sec_squared, kilometer_per_sec_squared, kilometer_per_min_per_sec, kilometer_per_hour_per_sec, kilometer_per_day_per_sec,
        galileo, knot_per_sec, inch_per_sec_squared, inch_per_min_per_sec, inch_per_hour_per_sec, inch_per_day_per_sec,
        foot_per_sec_squared, foot_per_min_per_sec, foot_per_hour_per_sec, foot_per_day_per_sec,
        mile_per_sec_squared, mile_per_min_per_sec, mile_per_hour_per_sec, mile_per_day_per_sec, standardGravity
    };

    /// <summary>
    /// Enumeration of available Angle conversions.
    /// </summary>
    public enum EnumConversionsAngle
    {
        radian, gradian, degree, arcminute, arcsecond, revolution, quadrant, sextant, octant, sign, pi
    };

    /// <summary>
    /// Enumeration of available Area conversions.
    /// </summary>
    public enum EnumConversionsArea
    {
        meter_squared, centimeter_squared, millimeter_squared, kilometer_squared, are, decare, hectare, dunam, stremma,
        inch_squared, foot_squared, yard_squared, mile_squared, section, acre, rood, township
    }

    /// <summary>
    /// Enumeration of available Area Moment First conversions.
    /// </summary>
    public enum EnumConversionsAreaMomentFirst
    {
        meter_cubed, decimeter_cubed, centimeter_cubed, millimeter_cubed, inch_cubed, foot_cubed
    }

    /// <summary>
    /// Enumeration of available Area Moment Second conversions.
    /// </summary>
    public enum EnumConversionsAreaMomentSecond
    {
        meter_quaded, decimeter_quaded, centimeter_quaded, millimeter_quaded, inch_quaded, foot_quaded
    }

    /// <summary>
    /// Enumeration of available Data conversions.
    /// </summary>
    public enum EnumConversionsData
    {
        // Keyword 'byte' is reserved by C# so use 'Byte' here as alternate.
        bit, kilobit, megabit, gigabit, terabit, petabit, exabit, zettabit, yottabit, nibble,
        octet, Byte, kilobyte, megabyte, gigabyte, terabyte, petabyte, exabyte, zettabyte, yottabyte,
        kibibit, mebibit, gibibit, tebibit, pebibit, exbibit, zebibit, yobibit,
        kibibyte, mebibyte, gibibyte, tebibyte, pebibyte, exbibyte, zebibyte, yobibyte
    }

    /// <summary>
    /// Enumeration of available Density conversions.
    /// </summary>
    public enum EnumConversionsDensity
    {
        kilogram_per_meter_cubed, gram_per_meter_cubed, milligram_per_meter_cubed, gram_per_centimeter_cubed,
        kilogram_per_liter, gram_per_milliliter, ounce_per_foot_cubed, ounce_per_inch_cubed, ounce_per_gallonUSfld, ounce_per_gallonIMP,
        pound_per_foot_cubed, pound_per_inch_cubed, pound_per_gallonUSfld, pound_per_gallonIMP, slug_per_foot_cubed
    };

    /// <summary>
    /// Enumeration of available Energy conversions.
    /// </summary>
    public enum EnumConversionsEnergy
    {
        joule, coulomb_volt, kilogram_meter_squared_per_sec_squared, newton_meter, pascal_meter_cubed, watt_sec, millijoule, microjoule,
        nanojoule, picojoule, kilojoule, megajoule, gigajoule, terajoule, erg, celsiusHeatUnitIT, dyne_centimeter, watt_hour, milliwatt_hour,
        microwatt_hour, kilowatt_hour, megawatt_hour, gigawatt_hour, terawatt_hour, petawatt_hour, kilogramForce_meter, kilogramForce_centimeter,
        gramForce_centimeter, ounceForce_inch, ounceForce_foot, poundForce_inch, poundForce_foot, poundal_inch, poundal_foot,
        horsepowerBoiler_hour, horsepowerElectric_hour, horsepowerMechanical_hour, horsepowerMetric_hour, tonCoalEquiv, tonOilEquiv, tonTNTEquiv,
        atmosphere_liter, atmosphere_meter_cubed, atmosphere_centimeter_cubed, atmosphere_inch_cubed, atmosphere_foot_cubed,
        calorieC15, calorieFood, calorieIT, calorieTH, kilocalorieC15, kilocalorieFood, kilocalorieIT, kilocalorieTH,
        thermieIT, btuC15, btuIMP, btuISO, btuIT, btuTH, thermC15, thermIMP, thermISO, thermIT, thermTH, quad, atomicMassUnit, electronvolt,
        hartree, kelvin, kilogram
    };

    /// <summary>
    /// Enumeration of available Flow conversions.
    /// </summary>
    public enum EnumConversionsFlow
    {
        meter_cubed_per_sec, meter_cubed_per_min, meter_cubed_per_hour, meter_cubed_per_day,
        decimeter_cubed_per_sec, decimeter_cubed_per_min, decimeter_cubed_per_hour, decimeter_cubed_per_day,
        centimeter_cubed_per_sec, centimeter_cubed_per_min, centimeter_cubed_per_hour, centimeter_cubed_per_day,
        millimeter_cubed_per_sec, millimeter_cubed_per_min, millimeter_cubed_per_hour, millimeter_cubed_per_day,
        liter_per_sec, liter_per_min, liter_per_hour, liter_per_day,
        milliliter_per_sec, milliliter_per_min, milliliter_per_hour, milliliter_per_day,
        microliter_per_sec, microliter_per_min, microliter_per_hour, microliter_per_day,
        inch_cubed_per_sec, inch_cubed_per_min, inch_cubed_per_hour, inch_cubed_per_day,
        foot_cubed_per_sec, foot_cubed_per_min, foot_cubed_per_hour, foot_cubed_per_day,
        yard_cubed_per_sec, yard_cubed_per_min, yard_cubed_per_hour, yard_cubed_per_day,
        acre_foot_per_sec, acre_foot_per_min, acre_foot_per_hour, acre_foot_per_day,
        gallonUSfldWine_per_sec, gallonUSfldWine_per_min, gallonUSfldWine_per_hour, gallonUSfldWine_per_day,
        ounceUSfld_per_sec, ounceUSfld_per_min, ounceUSfld_per_hour, ounceUSfld_per_day,
        barrelUSfldPetro_per_sec, barrelUSfldPetro_per_min, barrelUSfldPetro_per_hour, barrelUSfldPetro_per_day,
        gallonIMP_per_sec, gallonIMP_per_min, gallonIMP_per_hour, gallonIMP_per_day,
        ounceIMP_per_sec, ounceIMP_per_min, ounceIMP_per_hour, ounceIMP_per_day,
        barrelIMP_per_sec, barrelIMP_per_min, barrelIMP_per_hour, barrelIMP_per_day
    };

    /// <summary>
    /// Enumeration of available Force conversions.
    /// </summary>
    public enum EnumConversionsForce
    {
        newton, kilogram_meter_per_sec_squared, millinewton, micronewton, kilonewton, meganewton, dyne, kilogramForce, gramForce,
        poundForce, ounceForce, kipForce, poundal, tonForceUS, tonForceIMP
    };

    /// <summary>
    /// Enumeration of available Frequency conversions.
    /// </summary>
    public enum EnumConversionsFrequency
    {
        hertz, decihertz, centihertz, millihertz, microhertz, nanohertz, picohertz, femtohertz, attohertz, zeptohertz, yoctohertz,
        decahertz, hectohertz, kilohertz, megahertz, gigahertz, terahertz, petahertz, exahertz, zettahertz, yottahertz,
        cycle_per_sec, cycle_per_min, cycle_per_hour, cycle_per_day,
        revolution_per_sec, revolution_per_min, revolution_per_hour, revolution_per_day,
        radian_per_sec, radian_per_min, radian_per_hour, radian_per_day,
        degree_per_sec, degree_per_min, degree_per_hour, degree_per_day,
        wavelengthLight_meter, wavelengthLight_decimeter, wavelengthLight_centimeter, wavelengthLight_millimeter, wavelengthLight_micrometer
    };

    /// <summary>
    /// Enumeration of available Length conversions.
    /// </summary>
    public enum EnumConversionsLength
    {
        meter, decimeter, centimeter, millimeter, micrometer, nanometer, picometer, femtometer, attometer, zeptometer, yoctometer,
        decameter, hectometer, kilometer, megameter, gigameter, terameter, petameter, exameter, zettameter, yottameter, angstrom,
        inch, mil, handBase10, foot, rod, chain, furlong, yard, fathom, mile, nauticalMile, nauticalMileIMP, cable, astronomicalUnit,
        lightSecond, lightMinute, lightHour, lightDay, lightWeek, lightYear
    };

    /// <summary>
    /// Enumeration of available Mass conversions.
    /// </summary>
    public enum EnumConversionsMass
    {
        kilogram, gram, milligram, microgram, megagram, carat, poundMetric, ounce, dram, pound, grain, stone, quaterUS, quaterIMP,
        cental, hundredweightUS, hundredweightIMP, kip, tonUS, tonIMP, poundTroy, ounceTroy, pennyweight, slug
    };

    /// <summary>
    /// Enumeration of available Metric conversions.
    /// </summary>
    public enum EnumConversionsMetric
    {
        one, deci, centi, milli, micro, nano, pico, femto, atto, zepto, yocto, deca, hecto, kilo, mega, giga, tera, peta, exa, zetta, yotta
    }

    /// <summary>
    /// Enumeration of available Power conversions.
    /// </summary>
    public enum EnumConversionsPower
    {
        watt, joule_per_sec, newton_meter_per_sec, kilogram_meter_squared_per_sec_cubed, milliwatt, microwatt, nanowatt, picowatt, kilowatt, megawatt,
        gigawatt, terawatt, atmosphere_liter_per_sec, atmosphere_liter_per_min, atmosphere_liter_per_hour, atmosphere_meter_cubed_per_sec,
        atmosphere_meter_cubed_per_min, atmosphere_meter_cubed_per_hour, atmosphere_centimeter_cubed_per_sec, atmosphere_centimeter_cubed_per_min,
        atmosphere_centimeter_cubed_per_hour, atmosphere_inch_cubed_per_sec, atmosphere_inch_cubed_per_min, atmosphere_inch_cubed_per_hour,
        atmosphere_foot_cubed_per_sec, atmosphere_foot_cubed_per_min, atmosphere_foot_cubed_per_hour, lusec, 
        newton_meter_per_min, newton_meter_per_hour, gramForce_centimeter_per_sec, gramForce_centimeter_per_min, gramForce_centimeter_per_hour,
        kilogramForce_meter_per_sec, kilogramForce_meter_per_min, kilogramForce_meter_per_hour, poncelet, erg_per_sec, calorieIT_per_sec,
        calorieIT_per_min, calorieIT_per_hour, calorieTH_per_sec, calorieTH_per_min, calorieTH_per_hour, btuIT_per_sec, btuIT_per_min, btuIT_per_hour,
        foot_squared_equiv_direct_radiation, btuTH_per_sec, btuTH_per_min, btuTH_per_hour, ounceForce_foot_per_sec, ounceForce_foot_per_min,
        ounceForce_foot_per_hour, poundForce_foot_per_sec, poundForce_foot_per_min, poundForce_foot_per_hour, poundal_foot_per_sec, poundal_foot_per_min,
        poundal_foot_per_hour, horsepowerBoiler, horsepowerElectric, horsepowerMechanical, horsepowerMetric, refrigeration_pound, refrigeration_tonUS,
        refrigeration_tonIMP
    };

    /// <summary>
    /// Enumeration of available Pressure conversions.
    /// </summary>
    public enum EnumConversionsPressure
    {
        pascal, newton_per_meter_squared, kilogram_per_meter_per_sec_squared, decipascal, centipascal, millipascal, micropascal, hectopascal,
        kilopascal, megapascal, gigapascal, bar, millibar, microbar, barye, dyne_per_centimeter_squared, atmosphereStandard, atmosphereTechnical,
        torr, poundForce_per_inch_squared, poundForce_per_foot_squared, kipForce_per_inch_squared, kipForce_per_foot_squared,
        tonForceUS_per_foot_squared, tonForceIMP_per_foot_squared, poundal_per_inch_squared, poundal_per_foot_squared,
        kilogramForce_per_meter_squared, kilogramForce_per_centimeter_squared, kilogramForce_per_millimeter_squared,
        mercury_meter, mercury_centimeter, mercury_millimeter, mercury_inch, mercury_foot, waterC4_meter, waterC4_centimeter, waterC4_millimeter,
        waterC4_inch, waterC4_foot, waterC15_meter, waterC15_centimeter, waterC15_millimeter, waterC15_inch, waterC15_foot
    };

    /// <summary>
    /// Enumeration of available Speed conversions.
    /// </summary>
    public enum EnumConversionsSpeed
    {
        meter_per_sec, meter_per_min, meter_per_hour, meter_per_day, centimeter_per_sec, centimeter_per_min, centimeter_per_hour, centimeter_per_day,
        kilometer_per_sec, kilometer_per_min, kilometer_per_hour, kilometer_per_day, inch_per_sec, inch_per_min, inch_per_hour, inch_per_day,
        foot_per_sec, foot_per_min, foot_per_hour, foot_per_day, mile_per_sec, mile_per_min, mile_per_hour, mile_per_day, knot, knotIMP,
        speedMach, speedSound, speedLight
    };

    /// <summary>
    /// Enumeration of available Solid Angle conversions.
    /// </summary>
    public enum EnumConversionsSolidAngle
    {
        steradian, degree_squared, minute_squared, second_squared, hemisphere, sphere, spat
    };

    /// <summary>
    /// Enumeration of available Temperature conversions.
    /// </summary>
    public enum EnumConversionsTemperature
    {
        kelvin, celsius, fahrenheit, rankine
    };
    
    /// <summary>
    /// Enumeration of available Time conversions.
    /// </summary>
    public enum EnumConversionsTime
    {
        second, decisecond, centisecond, millisecond, microsecond, nanosecond, jiffy, moment, minute, hour, day, week, fortnight,
        year365, month365, year366, month366, yearJulian, monthJulian, decadeJulian, centuryJulian, millenniumJulian
    };

    /// <summary>
    /// Enumeration of available Torque conversions.
    /// </summary>
    public enum EnumConversionsTorque
    {
        newton_meter, newton_centimeter, millinewton_meter, micronewton_meter, kilonewton_meter, meganewton_meter, dyne_centimeter,
        kilopond_meter, kilogramForce_meter, kilogramForce_centimeter, gramForce_centimeter, ounceForce_inch, ounceForce_foot,
        poundForce_inch, poundForce_foot, kipForce_inch, kipForce_foot, poundal_inch, poundal_foot
    };

    /// <summary>
    /// Enumeration of available Volume conversions.
    /// </summary>
    public enum EnumConversionsVolume
    {
        meter_cubed, decimeter_cubed, centimeter_cubed, millimeter_cubed, micrometer_cubed, nanometer_cubed, liter, milliliter, microliter,
        inch_cubed, foot_cubed, yard_cubed, boardFoot, acre_foot, cord, gallonUSfldWine, quartUSfld, pintUSfld, cupUSfld, ounceUSfld,
        tablespoonUSfld, teaspoonUSfld, dramUSfld, minimUSfld, kegUSfldBeer, barrelUSfldPetro, gallonUSdry, quartUSdry, pintUSdry,
        peckUSdry, bushelUSdry, barrelUSdry, gallonIMP, quartIMP, pintIMP, gillIMP, ounceIMP, dramIMP, minimIMP, bushelIMP, barrelIMP, cranIMP
    };

    /* End of Conversion Enumerations ******************************************************************************************/


    /// <summary>
    /// LibUC = Shorthand for LibraryUnitConversions.
    /// </summary>
    public static class LibUC
    {
        /// <summary>
        /// Common prefix string of various enumerations used in library. Prefix must match common start of enumerations above.
        /// In some cases, this prefix will need to be removed from a result to calculate another result.
        /// Prefix is required for at least one method, GetListOfConversions().
        /// </summary>
        private static readonly string stringPrefixEnumConversions = "EnumConversions";

        /// <summary>
        /// Common decimal value used to initialize decimals in library. If a method returns a decimal and it is equal to this value,
        /// then error most likely occurred and calling method can throw an exception.
        /// </summary>
        private static readonly decimal decimalErrorValue = decimal.MinValue + 1.192837465m;

        /* Fundamental physical constants (fpc) ************************************************************************/

        #region
        // Many conversions that follow rely on fundamental physical constants.  These constants are often abbreviated with one or 
        // two characters or possibly with subscripts and/or superscripts.  In order to keep these constants grouped together and 
        // easy to use, their names will be proceeded by 'fpc_' indicating they are a 'fundamental physical constant'.  These
        // constant are latest values accepted by CODATA.  http://physics.nist.gov/cuu/Constants/index.html
        // Note: Will often will see other values with slight differences in various websites, publications, charts, apps, etc. 
        // This database attempts to use the latest CODATA values from referenced link above.  CODATA value may change over time.
        // Note: Many FPC's exceed Decimal limit due to high exponential values and are beyond scope of this Decimal database.

        // For consistency, sort fpc's by their name.

        // https://en.wikipedia.org/wiki/Light-second
        /// <summary>
        /// FPC light-second (c) = 299792458 meter/sec exact.
        /// </summary>
        public const decimal fpc_c = 299792458m;

        // Value of elementary charge = 1.6021766208E-19m exceeds maximum decimal digits and will be rounded to 1.602176621E-19m.
        // https://en.wikipedia.org/wiki/Elementary_charge
        // http://physics.nist.gov/cgi-bin/cuu/Convert?exp=0&num=&From=hr&To=TO&Action=Convert+value+and+show+factor
        /// <summary>
        /// FPC elementary charge (e) = 1.602176621E-19m coulombs to maximum decimal digits.
        /// </summary>
        public const decimal fpc_e = 1.6021766208E-19m;    // Value will be rounded by compiler.

        // https://en.wikipedia.org/wiki/Electronvolt
        // http://physics.nist.gov/cgi-bin/cuu/Convert?exp=0&num=&From=hr&To=TO&Action=Convert+value+and+show+factor
        /// <summary>
        /// FPC electronVolt (eV) = 1.602176621E-19 joules to maximum decimal digits.
        /// </summary>
        public const decimal fpc_eV = fpc_e;

        // https://en.wikipedia.org/wiki/Standard_gravity
        /// <summary>
        /// FPC standard gravity (g0) = 9.80665 meter/sec² exact.
        /// </summary>
        public const decimal fpc_g0 = 9.80665m;

        // https://en.wikipedia.org/wiki/Standard_gravity
        /// <summary>
        /// FPC standard gravity (g0-US) = 32.174048556430446194225721785 foot/sec² to maximum decimal digits.
        /// </summary>
        public const decimal fpc_g0_US = fpc_g0 / lengthFoot;

        // https://en.wikipedia.org/wiki/Hartree
        // http://physics.nist.gov/cgi-bin/cuu/Convert?exp=0&num=&From=hr&To=TO&Action=Convert+value+and+show+factor
        /// <summary>
        /// FPC hartree (Ha) = 4.35974465E-18 joules.
        /// </summary>
        public const decimal fpc_Ha = 4.35974465E-18m;

        // Value of Boltzmann constant = 1.38064852E-23 exceeds maximum decimal digits and will be rounded to 1.38065E-23.
        // https://en.wikipedia.org/wiki/Boltzmann_constant
        // http://physics.nist.gov/cgi-bin/cuu/Convert?exp=0&num=&From=hr&To=TO&Action=Convert+value+and+show+factor
        /// <summary>
        /// FPC Boltzmann constant (k) = 1.38065E-23 joules/kelvin to maximum decimal digits.
        /// </summary>
        public const decimal fpc_k = 1.38064852E-23m;    // Value will be rounded by compiler.

        // http://www.exploratorium.edu/fpc_pi/pi_archive/Pi10-6.html
        /// <summary>
        /// FPC 'pi' = 3.1415926535897932384626433833 to maximum decimal digits.
        /// </summary>
        public const decimal fpc_pi = 3.1415926535897932384626433833m;

        // http://physics.nist.gov/cgi-bin/cuu/Convert?exp=0&num=&From=hr&To=TO&Action=Convert+value+and+show+factor
        /// <summary>
        /// FPC atomic mass unit 'u' = 1.492418062E-10 joules.
        /// </summary>
        public const decimal fpc_u = 1.492418062E-10m;
        #endregion

        /* Conversion constants ****************************************************************************************/

        #region
        // The following is a public list of metric prefix constants availble for use by any calling method.
        // https://en.wikipedia.org/wiki/Metric_prefix

        /// <summary>
        /// Metric prefix: yotta (Y) = 1,000,000,000,000,000,000,000,000 = 10E+24.
        /// </summary>
        public const decimal mp_yotta = 1000000000000000000000000m;

        /// <summary>
        /// Metric prefix: zetta (Z) = 1,000,000,000,000,000,000,000 = 10E+21.
        /// </summary>
        public const decimal mp_zetta = 1000000000000000000000m;

        /// <summary>
        /// Metric prefix: exa (E) = 1,000,000,000,000,000,000 = 10E+18.
        /// </summary>
        public const decimal mp_exa = 1000000000000000000m;

        /// <summary>
        /// Metric prefix: peta (P) = 1,000,000,000,000,000 = 10E+15.
        /// </summary>
        public const decimal mp_peta = 1000000000000000m;

        /// <summary>
        /// Metric prefix: tera (T) = 1,000,000,000,000 = 10E+12.
        /// </summary>
        public const decimal mp_tera = 1000000000000m;

        /// <summary>
        /// Metric prefix: giga (G) = 1,000,000,000 = 10E+9.
        /// </summary>
        public const decimal mp_giga = 1000000000m;

        /// <summary>
        /// Metric prefix: mega (M) = 1,000,000 = 10E+6.
        /// </summary>
        public const decimal mp_mega = 1000000m;

        /// <summary>
        /// Metric prefix: kilo (k) = 1,000 = 10E+3.
        /// </summary>
        public const decimal mp_kilo = 1000m;

        /// <summary>
        /// Metric prefix: hecto (h) = 100 = 10E+2.
        /// </summary>
        public const decimal mp_hecto = 100m;

        /// <summary>
        /// Metric prefix: deca (da) = 10 = 10E+1.
        /// </summary>
        public const decimal mp_deca = 10m;

        /// <summary>
        /// Metric prefix: one = 1.
        /// </summary>
        public const decimal mp_one = 1m;

        /// <summary>
        /// Metric prefix: deci (d) = 0.1 = 10E-1.
        /// </summary>
        public const decimal mp_deci = 0.1m;

        /// <summary>
        /// Metric prefix: centi (c) = 0.01 = 10E-2.
        /// </summary>
        public const decimal mp_centi = 0.01m;

        /// <summary>
        /// Metric prefix: milli (m) = 0.001 = 10E-3.
        /// </summary>
        public const decimal mp_milli = 0.001m;

        /// <summary>
        /// Metric prefix: micro (μ) =  0.000001 = 10E-6.
        /// </summary>
        public const decimal mp_micro = 0.000001m;

        /// <summary>
        /// Metric prefix: nano (n) = 0.000000001= 10E-9.
        /// </summary>
        public const decimal mp_nano = 0.000000001m;

        /// <summary>
        /// Metric prefix: pico (p) = 0.000000000001 = 10E-12.
        /// </summary>
        public const decimal mp_pico = 0.000000000001m;

        /// <summary>
        /// Metric prefix: femto (f) = 0.000000000000001 = 10E-15.
        /// </summary>
        public const decimal mp_femto = 0.000000000000001m;

        /// <summary>
        /// Metric prefix: atto (a) = 0.000000000000000001 = 10E-18.
        /// </summary>
        public const decimal mp_atto = 0.000000000000000001m;

        /// <summary>
        /// Metric prefix: zepto (z) = 0.000000000000000000001 = 10E-21.
        /// </summary>
        public const decimal mp_zepto = 0.000000000000000000001m;

        /// <summary>
        /// Metric prefix: yocto (y) = 0.000000000000000000000001 = 10E-24.
        /// </summary>
        public const decimal mp_yocto = 0.000000000000000000000001m;

        // The following is a list of private constants used to create the various lists used in this database.
        // If these values are not exact, then they are carried out to maximum decimal limit. Considerable testing has 
        // verified that using several of these variables in basic math (+, -, *, /) results in no precision loss.
        // Conclusion:  Compiler uses higher accuracy than the decimal limit when it calculates these constant values!

        /// <summary>
        /// 1 are = 100 meters² exact.
        /// </summary>
        private const decimal areaAre = 100m;

        /// <summary>
        /// 1 foot² = 0.09290304 meters² exact.
        /// </summary>
        private const decimal areaFootSquared = lengthFoot * lengthFoot;

        /// <summary>
        /// 1 inch² = 0.00064516 meters² exact.
        /// </summary>
        private const decimal areaInchSquared = lengthInch * lengthInch;

        /// <summary>
        /// 1 mile² = 2589988.110336 meters² exact.
        /// </summary>
        private const decimal areaMileSquared = lengthMile * lengthMile;

        /// <summary>
        /// 1 yard² = 0.83612736 meters² exact.
        /// </summary>
        private const decimal areaYardSquared = lengthYard * lengthYard;

        // https://en.wikipedia.org/wiki/Millimeter_of_mercury
        /// <summary>
        /// 1 millimeter of mercury = 13595.1 kilogram/meter³ exact.
        /// </summary>
        private const decimal densityMercury = 13595.1m;

        // There are two common measurements for water, 4 degrees Celsius (39.2 F) and 15 degrees Celsius (59.0 F).  
        // Often 60 degrees Fahrenheit, which is exactly 15.55555555555555555555555556 Celsius to decimal limit, is 
        // used or seen in various websites, publications, charts, data, etc.  For consistency, this databases uses 
        // the 15 degrees Celsius value versus the 60 degree Fahrenheit value.

        // https://en.wikipedia.org/wiki/Centimetre_of_water
        /// <summary>
        /// 1 centimeter of water at 4 deg C = 999.9720 kilogram/meter³ exact.
        /// </summary>
        private const decimal densityWaterC4 = 999.9720m;

        // https://en.wikipedia.org/wiki/Centimetre_of_water
        /// <summary>
        /// 1 centimeter of water at 15 deg C = 1000.0 kilogram/meter³ exact.
        /// </summary>
        private const decimal densityWaterC15 = 1000m;

        // https://en.wikipedia.org/wiki/Calorie
        /// <summary>
        /// 1 calorie-C15 = 4.1858 joules (not necessarily exact value but common value).
        /// </summary>
        private const decimal energyCalorieC15 = 4.1858m;

        // https://en.wikipedia.org/wiki/Calorie
        /// <summary>
        /// 1 calorie-IT = 4.1868 joules exact.
        /// </summary>
        private const decimal energyCalorieIT = 4.1868m;

        // https://en.wikipedia.org/wiki/Calorie
        /// <summary>
        /// 1 calorie-TH = 4.184 joules exact = 0.001 food calorie exact.
        /// </summary>
        private const decimal energyCalorieTH = 4.184m;

        // Note about BTU: ISO now defines the TH calorie as exactly 4.184J and the IT calorie as exactly 4.1868J. 
        // BTU is then derived using conversions from grams to pounds and from Celsius to Fahrenheit.

        // http://www.wikidoc.org/index.php/British_thermal_unit
        /// <summary>
        /// 1 BTU-C15T = 1054.804 joules exact.
        /// </summary>
        private const decimal energyBtuC15 = 1054.804m;

        // https://en.wikipedia.org/wiki/British_thermal_unit
        /// <summary>
        /// 1 BTU-IMP = 1055.05585257348 joules exact.
        /// </summary>
        private const decimal energyBtuIMP = 1055.05585257348m;

        // http://www.wikidoc.org/index.php/British_thermal_unit
        /// <summary>
        /// 1 BTU-ISO = 1055.056 joules exact.
        /// </summary>
        private const decimal energyBtuISO = 1055.056m;

        // http://www.wikidoc.org/index.php/British_thermal_unit
        /// <summary>
        /// 1 BTU-IT = 1055.05585262 joules exact.
        /// </summary>
        private const decimal energyBtuIT = energyCalorieIT * 100m / 180m * (forcePound / (forceKilogram * massGram));

        // http://www.wikidoc.org/index.php/British_thermal_unit
        /// <summary>
        /// 1 BTU-TH = 1054.3502644888888888888888889 joules to maximum decimal digits.
        /// </summary>
        private const decimal energyBtuTH = energyCalorieTH * 100m / 180m * (forcePound / (forceKilogram * massGram));

        // Energy = E = mc² = k•T where k = Boltzmann constant and T = 1 degree Kelvin.
        // http://physics.nist.gov/cgi-bin/cuu/Convert?exp=0&num=&From=hr&To=TO&Action=Convert+value+and+show+factor
        /// <summary>
        /// 1 kelvin = 1.38065E-23 joules.
        /// </summary>
        private const decimal energyKelvin = fpc_k;

        // https://en.wikipedia.org/wiki/Dyne
        /// <summary>
        /// 1 dyne = 0.00001 newton exact.
        /// </summary>
        private const decimal forceDyne = 0.00001m;

        // https://en.wikipedia.org/wiki/Kilogram-force
        /// <summary>
        /// 1 kilogram-force = 9.80665 newton exact.
        /// </summary>
        private const decimal forceKilogram = fpc_g0;

        /// <summary>
        /// 1 ounce-force = 0.27801385095378125 newton exact.
        /// </summary>
        private const decimal forceOunce = forcePound / 16m;

        // "https://en.wikipedia.org/wiki/Pound_(force)"
        /// <summary>
        /// 1 pound-force = 4.4482216152605 newton exact.
        /// </summary>
        private const decimal forcePound = massPound * fpc_g0;

        // https://en.wikipedia.org/wiki/Poundal
        /// <summary>
        /// 1 poundal = 0.138254954376 newton exact.
        /// </summary>
        private const decimal forcePoundal = massPound * lengthFoot;

        // "https://en.wikipedia.org/wiki/Foot_(unit)"
        /// <summary>
        /// 1 foot = 0.3048 meter exact.
        /// </summary>
        private const decimal lengthFoot = lengthInch * 12m;

        // "https://en.wikipedia.org/wiki/Inch
        /// <summary>
        /// 1 inch = 0.0254 meter exact.
        /// </summary>
        private const decimal lengthInch = 0.0254m;

        // https://en.wikipedia.org/wiki/Mile
        /// <summary>
        /// 1 mile = 1609.344 meter exact.
        /// </summary>
        private const decimal lengthMile = lengthFoot * 5280m;

        // https://en.wikipedia.org/wiki/Nautical_mile
        /// <summary>
        /// 1 nautical mile = 1852 meter exact.
        /// </summary>
        private const decimal lengthNauticalMile = 1852m;

        // https://en.wikipedia.org/wiki/Nautical_mile
        /// <summary>
        /// 1 nautical mile IMP = 6080 foot = 1853.184 meter exact.
        /// </summary>
        private const decimal lengthNauticalMileIMP = 6080m * lengthFoot;

        // https://en.wikipedia.org/wiki/Yard
        /// <summary>
        /// 1 yard = 0.9144 meter exact.
        /// </summary>
        private const decimal lengthYard = lengthFoot * 3m;

        // "https://en.wikipedia.org/wiki/Pound_(mass)"
        /// <summary>
        /// 1 gram = 0.001 kilogram exact.
        /// </summary>
        private const decimal massGram = 0.001m;

        // "https://en.wikipedia.org/wiki/Pound_(mass)"
        /// <summary>
        /// 1 pound = 0.45359237 kilogram exact.
        /// </summary>
        private const decimal massPound = 0.45359237m;

        // https://en.wikipedia.org/wiki/Pound_(mass)#Troy_pound
        /// <summary>
        /// 1 pound troy = 0.3732417216 kilogram exact.
        /// </summary>
        private const decimal massPoundTroy = 0.3732417216m;

        /// <summary>
        /// 1 ounce = 0.028349523125 kilogram exact.
        /// </summary>
        private const decimal massOunce = massPound / 16m;

        // "https://en.wikipedia.org/wiki/Slug_(mass)"
        /// <summary>
        /// 1 slug = 14.593902937206364829396325459 kilogram to maximum decimal digits.
        /// </summary>
        private const decimal massSlug = forcePound / lengthFoot;

        // http://www.aqua-calc.com/what-is/power/boiler-horsepower
        // https://www.nist.gov/physical-measurement-laboratory/nist-guide-si-appendix-b8
        /// <summary>
        /// 1 horsepower boiler = 9810.554074015138888888888889 watt.  NIST value is different and equals 9809.5 watt.
        /// </summary>
        private const decimal powerHorsepowerBoiler = 33475m * energyBtuIT / timeHour;  // Valid calculation, equals 9810.554074015138888888888889 watts.

        // https://en.wikipedia.org/wiki/Horsepower#Electrical_horsepower
        /// <summary>
        /// 1 horsepower electric = 746 watt exact.
        /// </summary>
        private const decimal powerHorsepowerElectric = 746m;

        // https://en.wikipedia.org/wiki/Horsepower#Mechanical_horsepower
        /// <summary>
        /// 1 horsepower mechanical = 745.69987158227022 watt exact = 550 foot-pound/sec.
        /// </summary>
        private const decimal powerHorsepowerMechanical = 550m * lengthFoot * forcePound;

        // https://en.wikipedia.org/wiki/Horsepower#Metric_horsepower_.28PS.2C_cv.2C_hk.2C_pk.2C_ks.2C_ch.29
        /// <summary>
        /// 1 horsepower metric = 735.49875 watt exact.
        /// </summary>
        private const decimal powerHorsepowerMetric = 75m * fpc_g0;     // 75 kilograms times standard gravity.

        // https://en.wikipedia.org/wiki/Ton_of_refrigeration
        /// <summary>
        /// 1 refrigeration pound =  1.7584264210333333333333333334 watt to maximum decimal digits.
        /// </summary>
        private const decimal powerRefrigerationPound = energyBtuIT * 12000m * 24m / timeDay / 2000m;

        // https://en.wikipedia.org/wiki/Atmosphere_(unit)
        /// <summary>
        /// 1 atmosphere = 101325 pascal exact.
        /// </summary>
        private const decimal pressureAtmosphere = 101325m;

        // https://en.wikipedia.org/wiki/Bar_(unit)#Usage
        /// <summary>
        /// 1 bar = 100000 pascal exact.
        /// </summary>
        private const decimal pressureBar = 100000m;

        // https://en.wikipedia.org/wiki/Millimeter_of_mercury
        /// <summary>
        /// 1 meter of mercury = 133322.387415 pascal exact.
        /// </summary>
        private const decimal pressureMercury = densityMercury * fpc_g0;

        // There are two common measurements for water, 4 degrees Celsius (39.2 F) and 15 degrees Celsius (~60F).

        // https://en.wikipedia.org/wiki/Centimetre_of_water
        /// <summary>
        /// 1 meter of water at 4 deg C = 9806.3754138 pascal exact.
        /// </summary>
        private const decimal pressureWaterC4 = densityWaterC4 * fpc_g0;

        // https://en.wikipedia.org/wiki/Centimetre_of_water
        /// <summary>
        /// 1 meter of water at 15 deg C = 9806.65 pascal exact.
        /// </summary>
        private const decimal pressureWaterC15 = densityWaterC15 * fpc_g0;

        // "https://en.wikipedia.org/wiki/Knot_(unit)"
        /// <summary>
        /// 1 knot = 1852 meter/hour exact = 0.5144444444444444444444444444 meter/sec to maximum decimal digits.
        /// </summary>
        private const decimal speedKnot = lengthNauticalMile / timeHour;

        // "https://en.wikipedia.org/wiki/Knot_(unit)"
        /// <summary>
        /// 1 knot IMP = 1853.184 meter/hour exact = 0.5147733333333333333333333333 meter/sec to maximum decimal digits.
        /// </summary>
        private const decimal speedKnotIMP = lengthNauticalMileIMP / timeHour;

        /// <summary>
        /// 1 day = 86400 second exact.
        /// </summary>
        private const decimal timeDay = timeHour * 24m;

        /// <summary>
        /// 1 hour = 3600 second exact.
        /// </summary>
        private const decimal timeHour = timeMin * 60m;

        /// <summary>
        /// 1 minute = 60 second exact.
        /// </summary>
        private const decimal timeMin = 60m;

        // Used often so created variable.  Do not need variables for all metric cases.
        /// <summary>
        /// 1 centimeter³ = 0.000001 meters³ exact.
        /// </summary>
        private const decimal volumeCentimeterCubed = mp_centi * mp_centi * mp_centi;

        /// <summary>
        /// 1 foot³ = 0.028316846592 meters³ exact.
        /// </summary>
        private const decimal volumeFootCubed = lengthFoot * lengthFoot * lengthFoot;

        // https://en.wikipedia.org/wiki/Gallon
        /// <summary>
        /// 1 gallon IMP = 0.00454609 meters³ exact.
        /// </summary>
        private const decimal volumeGallonIMP = volumeLiter * 4.54609m;

        // https://en.wikipedia.org/wiki/Gallon
        /// <summary>
        /// 1 gallon US dry = 0.00440488377086 meters³ exact.
        /// </summary>
        private const decimal volumeGallonUSd = volumeInchCubed * 268.8025m;

        // https://en.wikipedia.org/wiki/Gallon
        /// <summary>
        /// 1 gallon US fluid = 0.003785411784 meters³ exact.
        /// </summary>
        private const decimal volumeGallonUSf = volumeInchCubed * 231m;

        /// <summary>
        /// 1 inch³ = 0.000016387064 meters³ exact.
        /// </summary>
        private const decimal volumeInchCubed = lengthInch * lengthInch * lengthInch;

        // https://en.wikipedia.org/wiki/Litre
        /// <summary>
        /// 1 liter = 0.001 meters³ exact.
        /// </summary>
        private const decimal volumeLiter = 0.001m;

        /// <summary>
        /// 1 yard³ = 0.764554857984 meters³ exact.
        /// </summary>
        private const decimal volumeYardCubed = lengthYard * lengthYard * lengthYard;
        #endregion

        
        /* Public library methods follow ***************************************************************************************/

        
        
        // Will need to update switch statement in this method if any new values added to EnumConversionsType.
        /// <summary>
        /// Returns void but sets out values listLibUCBase and StringHyperlinkConversion corresponding to parameter enumConversionsType.
        /// </summary>
        /// <param name="enumConversionsType">Enumeration of available conversion types. Samples: EnumConversionsType.Area, EnumConversionsType.Flow, EnumConversionsType.Length.</param>
        /// <param name="listLibUCBase">List to place available unit conversions corresponding to enumConversionsType.</param>
        /// <param name="StringHyperlinkConversion">String to place web link that provides information about enumConversionsType.</param>
        public static void GetConversionValues(EnumConversionsType enumConversionsType, out List<LibUCBase> listLibUCBase, out string StringHyperlinkConversion)
        {
            switch (enumConversionsType)
            {
                // These cases do not actually copy the list.  Efficient since sets pointer to existing conversion list. 
                case EnumConversionsType.Acceleration:
                    listLibUCBase = listConversionsAcceleration;
                    StringHyperlinkConversion = StringHyperlinkAcceleration;
                    break;
                case EnumConversionsType.Angle:
                    listLibUCBase = LibUC.listConversionsAngle;
                    StringHyperlinkConversion = StringHyperlinkAngle;
                    break;
                case EnumConversionsType.Area:
                    listLibUCBase = LibUC.listConversionsArea;
                    StringHyperlinkConversion = StringHyperlinkArea;
                    break;
                case EnumConversionsType.AreaMomentFirst:
                    listLibUCBase = LibUC.listConversionsAreaMomentFirst;
                    StringHyperlinkConversion = StringHyperlinkAreaMomentFirst;
                    break;
                case EnumConversionsType.AreaMomentSecond:
                    listLibUCBase = LibUC.listConversionsAreaMomentSecond;
                    StringHyperlinkConversion = StringHyperlinkAreaMomentSecond;
                    break;
                case EnumConversionsType.Data:
                    listLibUCBase = LibUC.listConversionsData;
                    StringHyperlinkConversion = StringHyperlinkData;
                    break;
                case EnumConversionsType.Density:
                    listLibUCBase = LibUC.listConversionsDensity;
                    StringHyperlinkConversion = StringHyperlinkDensity;
                    break;
                case EnumConversionsType.Energy:
                    listLibUCBase = LibUC.listConversionsEnergy;
                    StringHyperlinkConversion = StringHyperlinkEnergy;
                    break;
                case EnumConversionsType.Flow:
                    listLibUCBase = LibUC.listConversionsFlow;
                    StringHyperlinkConversion = StringHyperlinkFlow;
                    break;
                case EnumConversionsType.Force:
                    listLibUCBase = LibUC.listConversionsForce;
                    StringHyperlinkConversion = StringHyperlinkForce;
                    break;
                case EnumConversionsType.Frequency:
                    listLibUCBase = LibUC.listConversionsFrequency;
                    StringHyperlinkConversion = StringHyperlinkFrequency;
                    break;
                case EnumConversionsType.Length:
                    listLibUCBase = LibUC.listConversionsLength;
                    StringHyperlinkConversion = StringHyperlinkLength;
                    break;
                case EnumConversionsType.Mass:
                    listLibUCBase = LibUC.listConversionsMass;
                    StringHyperlinkConversion = StringHyperlinkMass;
                    break;
                case EnumConversionsType.Metric:
                    listLibUCBase = LibUC.listConversionsMetric;
                    StringHyperlinkConversion = StringHyperlinkMetric;
                    break;
                case EnumConversionsType.Power:
                    listLibUCBase = LibUC.listConversionsPower;
                    StringHyperlinkConversion = StringHyperlinkPower;
                    break;
                case EnumConversionsType.Pressure:
                    listLibUCBase = LibUC.listConversionsPressure;
                    StringHyperlinkConversion = StringHyperlinkPressure;
                    break;
                case EnumConversionsType.Speed:
                    listLibUCBase = LibUC.listConversionsSpeed;
                    StringHyperlinkConversion = StringHyperlinkSpeed;
                    break;
                case EnumConversionsType.SolidAngle:
                    listLibUCBase = LibUC.listConversionsSolidAngle;
                    StringHyperlinkConversion = StringHyperlinkSolidAngle;
                    break;
                    // Temperature conversions are special case since cannot be completed with simple mutiplication factor.
                    // They also require subtraction and addition. 
                    // Use conversion methods ConvertTemperatureToKelvin() and ConvertTemperatureFromKelvin() to convert values.
                    // All DecimalBase values set to 1m and are not used.
                case EnumConversionsType.Temperature:
                    listLibUCBase = LibUC.listConversionsTemperature;
                    StringHyperlinkConversion = StringHyperlinkTemperature;
                    break;
                case EnumConversionsType.Time:
                    listLibUCBase = LibUC.listConversionsTime;
                    StringHyperlinkConversion = StringHyperlinkTime;
                    break;
                case EnumConversionsType.Torque:
                    listLibUCBase = LibUC.listConversionsTorque;
                    StringHyperlinkConversion = StringHyperlinkTorque;
                    break;
                case EnumConversionsType.Volume:
                    listLibUCBase = LibUC.listConversionsVolume;
                    StringHyperlinkConversion = StringHyperlinkVolume;
                    break;
                default:    // Throw exception so error can be discovered and corrected.
                    throw new NotSupportedException($"LibUC.GetConversionValues(): Match for enumConversionsType={enumConversionsType} not found in switch statement.");
            }
            // Debug.WriteLine($"LibUC.GetConversionValues(): enumConversionsType={enumConversionsType}, listOfConversions[0]={listLibUCBase[0]}, stringURLAboutConversion={StringHyperlinkConversion}");
        }

        /// <summary>
        /// Return converted value of decimalValueIn (Decimal overload).
        /// Sample: decimalValueOut = LibUC.ConvertValue(1m, EnumConversionsLength.meter, EnumConversionsLength.foot);
        /// </summary>
        /// <typeparam name="TEnum">Generic enumeration.</typeparam>
        /// <param name="decimalValueIn">Decimal value to convert.</param>
        /// <param name="unitIn">Conversion enum input value. Samples: EnumConversionsArea.acre, EnumConversionsVolume.liter, EnumConversionsLength.foot.</param>
        /// <param name="unitOut">Conversion enum output value. Samples same as unitIn.</param>
        /// <returns></returns>
        public static decimal ConvertValue<TEnum>(decimal decimalValueIn, TEnum unitIn, TEnum unitOut) where TEnum : IComparable, IFormattable, IConvertible
        {
            // Debug.WriteLine($"LibUC.ConvertValue(): (Decimal overload): decimalValueIn={decimalValueIn}, unitIn={unitIn}, unitOut={unitOut}");
            ConversionSetup(unitIn, unitOut, out bool boolTemperatureConvert, out decimal decimalBaseUnitIn, out decimal decimalBaseUnitOut, out EnumConversionsTemperature tempIn, out EnumConversionsTemperature tempOut);
            try
            {
                if (boolTemperatureConvert)
                    return ConvertTemperatureFromKelvin(ConvertTemperatureToKelvin(decimalValueIn, tempIn), tempOut);  // Calculate the Temperature conversion.
                else
                    return (decimalValueIn * decimalBaseUnitIn / decimalBaseUnitOut);    // Calculate the conversion.
            }
            catch (OverflowException)
            {
                throw;  // Pass exception to calling method to be handled there.
            }
        }

        /// <summary>
        /// Return converted value of doubleValueIn (Double overload).
        /// Sample: doubleValueOut = LibUC.ConvertValue(1d, EnumConversionsLength.meter, EnumConversionsLength.foot);
        /// </summary>
        /// <typeparam name="TEnum">Generic enumeration.</typeparam>
        /// <param name="doubleValueIn">Double value to convert.</param>
        /// <param name="unitIn">Conversion enum input value. Samples: EnumConversionsArea.acre, EnumConversionsVolume.liter, EnumConversionsLength.foot.</param>
        /// <param name="unitOut">Conversion enum output value. Samples same as unitIn.</param>
        /// <returns></returns>
        public static double ConvertValue<TEnum>(double doubleValueIn, TEnum unitIn, TEnum unitOut) where TEnum : IComparable, IFormattable, IConvertible
        {
            // Debug.WriteLine($"LibUC.ConvertValue(): (Double overload): doubleValueIn={doubleValueIn}, unitIn={unitIn}, unitOut={unitOut}");
            ConversionSetup(unitIn, unitOut, out bool boolTemperatureConvert, out decimal decimalBaseUnitIn, out decimal decimalBaseUnitOut, out EnumConversionsTemperature tempIn, out EnumConversionsTemperature tempOut);
            try
            {
                if (boolTemperatureConvert)
                    return (double)ConvertTemperatureFromKelvin(ConvertTemperatureToKelvin((decimal)doubleValueIn, tempIn), tempOut);  // Calculate the Temperature conversion.
                else
                    return (double)((decimal)doubleValueIn * decimalBaseUnitIn / decimalBaseUnitOut);    // Calculate the conversion.
            }
            catch (OverflowException)
            {
                throw;  // Pass exception to calling method to be handled there.
            }
        }

        /// <summary>
        /// Return converted value of floatValueIn (Float overload).
        /// Sample: floatValueOut = LibUC.ConvertValue(1f, EnumConversionsLength.meter, EnumConversionsLength.foot);
        /// </summary>
        /// <typeparam name="TEnum">Generic enumeration.</typeparam>
        /// <param name="floatValueIn">Float value to convert.</param>
        /// <param name="unitIn">Conversion enum input value. Samples: EnumConversionsArea.acre, EnumConversionsVolume.liter, EnumConversionsLength.foot.</param>
        /// <param name="unitOut">Conversion enum output value. Samples same as unitIn.</param>
        /// <returns></returns>
        public static float ConvertValue<TEnum>(float floatValueIn, TEnum unitIn, TEnum unitOut) where TEnum : IComparable, IFormattable, IConvertible
        {
            // Debug.WriteLine($"LibUC.ConvertValue(): (Float overload): floatValueIn={floatValueIn}, unitIn={unitIn}, unitOut={unitOut}");
            ConversionSetup(unitIn, unitOut, out bool boolTemperatureConvert, out decimal decimalBaseUnitIn, out decimal decimalBaseUnitOut, out EnumConversionsTemperature tempIn, out EnumConversionsTemperature tempOut);
            try
            {
                if (boolTemperatureConvert)
                    return (float)ConvertTemperatureFromKelvin(ConvertTemperatureToKelvin((decimal)floatValueIn, tempIn), tempOut);  // Calculate the Temperature conversion.
                else
                    return (float)((decimal)floatValueIn * decimalBaseUnitIn / decimalBaseUnitOut);    // Calculate the conversion.
            }
            catch (OverflowException)
            {
                throw;      // Pass exception to calling method to be handled there.
            }
        }

        /// <summary>
        /// Common code called from ConvertValue() decimal, double, and float overload methods. Returns void but sets various out parameters.
        /// </summary>
        /// <typeparam name="TEnum">Generic enumeration.</typeparam>
        /// <param name="unitIn">Conversion enum input value. Samples: EnumConversionsTemperature.celsius, EnumConversionsArea.acre, EnumConversionsVolume.liter, EnumConversionsLength.foot.</param>
        /// <param name="unitOut">Conversion enum output value. Samples same as unitIn.</param>
        /// <param name="boolTemperatureConvert">True if unitIn is EnumConversionsTemperature.XXX, false otherwise.</param>
        /// <param name="decimalBaseUnitIn">Conversion factor of unitIn relative to base unit.</param>
        /// <param name="decimalBaseUnitOut">Conversion factor of unitOut relative to base unit.</param>
        /// <param name="tempIn">If generic unitIn is same as EnumConversionsTemperature.XXX, then convert unitIn to specific EnumConversionsTemperature.XXX.</param>
        /// <param name="tempOut">If generic unitOut is same as EnumConversionsTemperature.XXX, then convert unitOut to specific EnumConversionsTemperature.XXX.</param>
        private static void ConversionSetup<TEnum>(TEnum unitIn, TEnum unitOut, out bool boolTemperatureConvert, out decimal decimalBaseUnitIn, out decimal decimalBaseUnitOut, out EnumConversionsTemperature tempIn, out EnumConversionsTemperature tempOut) where TEnum : IComparable, IFormattable, IConvertible
        {
            // Initialize out parameter values.
            boolTemperatureConvert = false;
            decimalBaseUnitIn = 1m;
            decimalBaseUnitOut = decimalBaseUnitIn;
            tempIn = EnumConversionsTemperature.kelvin;
            tempOut = tempIn;
            string stringEnumConversionsFound = unitIn.GetType().Name;
            string stringEnumConversionsTemperature = tempIn.GetType().Name;
            // Debug.WriteLine($"LibUC.ConversionSetup(): unitIn={unitIn}, unitOut={unitOut}, stringEnumConversionsFound={stringEnumConversionsFound}, stringEnumConversionsTemperature={stringEnumConversionsTemperature}");
            if (stringEnumConversionsFound.Equals(stringEnumConversionsTemperature))
            {
                // Setup conversion values for Temperature conversion.
                boolTemperatureConvert = true;  // Conversion type is Temperature so set true.
                if (!Enum.TryParse(unitIn.ToString(), out tempIn))          // Convert generic unitIn to equivalent EnumConversionsTemperature.XXX.
                {
                    // Throw exception so error can be discovered and corrected.
                    throw new ArgumentOutOfRangeException($"LibUC.ConversionSetup(): unitIn={unitIn} not found in Temperature.");
                }
                else
                {
                    if (!Enum.TryParse(unitOut.ToString(), out tempOut))    // Convert generic unitOut to equivalent EnumConversionsTemperature.XXX.
                    {
                        // Throw exception so error can be discovered and corrected.
                        throw new ArgumentOutOfRangeException($"LibUC.ConversionSetup(): unitOut={unitOut} not found in Temperature.");
                    }
                }
                // Debug.WriteLine($"LibUC.ConversionSetup(): boolTemperatureConvert={boolTemperatureConvert}, tempIn={tempIn}, tempOut={tempOut}");
            }
            else
            {
                // Setup conversion factors for every conversion but Temperature conversion.
                decimalBaseUnitIn = GetDecimalBase(unitIn);
                if (decimalBaseUnitIn.Equals(decimalErrorValue))
                {
                    // Throw exception so error can be discovered and corrected.
                    throw new ArgumentOutOfRangeException($"LibUC.ConversionSetup(): Did not find match in unitIn={unitIn}.");
                }
                decimalBaseUnitOut = GetDecimalBase(unitOut);
                if (decimalBaseUnitIn.Equals(decimalErrorValue))
                {
                    // Throw exception so error can be discovered and corrected.
                    throw new ArgumentOutOfRangeException($"LibUC.ConversionSetup(): Did not find match in unitOut={unitOut}.");
                }
                // Debug.WriteLine($"LibUC.ConversionSetup(): decimalBaseUnitIn={decimalBaseUnitIn}, decimalBaseUnitOut={decimalBaseUnitOut}");
            }
        }

        /// <summary>
        /// Returns DecimalBase corresponding to parameter enumConversions. DecimalBase is conversion factor of enumConversions relative to base unit.
        /// </summary>
        /// <typeparam name="TEnum">Generic enumeration.</typeparam>
        /// <param name="enumConversions">Enum of available conversions. Samples: EnumConversionsArea.acre, EnumConversionsVolume.liter, EnumConversionsLength.foot.</param>
        /// <returns>Unit conversion.</returns>
        public static decimal GetDecimalBase<TEnum>(TEnum enumConversions) where TEnum : IComparable, IFormattable, IConvertible
        {
            // Debug.WriteLine($"LibUCGetDecimalBase(): Starting method, enumConversions={enumConversions}");
            _ = new List<LibUCBase>();      // List Samples: listConversionsArea, listConversionsVolume, listConversionsLength.
            List<LibUCBase> listLibUCBase = GetListOfConversions(enumConversions);
            decimal decimalBase = decimalErrorValue;
            foreach (LibUCBase libUCBase in listLibUCBase)   // Compare each libUCBase in list until match found, then return DecimalBase.
            {
                if (enumConversions.Equals(libUCBase.EnumConversions))
                    return libUCBase.DecimalBase;    // Found match so exit.
            }
            return decimalBase;     // Error: Did not find match so return decimalErrorValue.
        }

        /// <summary>
        /// Return list of available conversions that contains parameter enumConversions (TEnum enumConversions overload).
        /// </summary>
        /// <typeparam name="TEnum">Generic Enum.</typeparam>
        /// <param name="enumConversions">Enum of available conversions. Samples: EnumConversionsArea.acre, EnumConversionsVolume.liter, EnumConversionsLength.foot.</param>
        /// <returns></returns>
        public static List<LibUCBase> GetListOfConversions<TEnum>(TEnum enumConversions) where TEnum : IComparable, IFormattable, IConvertible
        {
            string stringEnumConversions = enumConversions.GetType().Name;  // Sample: EnumConversionsLength.foot returns EnumConversionsLength.
            string stringEnumConversionsType = stringEnumConversions.Substring(stringPrefixEnumConversions.Length);     // Sample: EnumConversionsLength returns Length.
            // Debug.WriteLine($"LibUC.GetListOfConversions(): enumConversions={enumConversions}, stringEnumConversions={stringEnumConversions}, stringEnumConversionsType={stringEnumConversionsType}");
            // From stringEnumConversionsType, derive parent enumConversionsType. Samples: Area, Flow, Length.
            if (!Enum.TryParse(stringEnumConversionsType, out EnumConversionsType enumConversionsType))
            {
                // Throw exception so error can be discovered and corrected.
                throw new ArgumentOutOfRangeException($"LibUC.GetConversionList(): stringEnumConversionsType={stringEnumConversionsType} not valid.");
            }
            // Debug.WriteLine($"LibUC.GetListOfConversions(): Found enumConversionsType={enumConversionsType}");
            return GetListOfConversions(enumConversionsType);
        }

        /// <summary>
        /// Return list of available conversions corresponding to parameter enumConversionsType (EnumConversionsType overload).
        /// </summary>
        /// <param name="enumConversionsType">Enumeration of available conversion types. Samples: EnumConversionsType.Area, EnumConversionsType.Flow, EnumConversionsType.Length.</param>
        /// <returns></returns>
        public static List<LibUCBase> GetListOfConversions(EnumConversionsType enumConversionsType)
        {
            GetConversionValues(enumConversionsType, out List<LibUCBase> listLibUCBase, out _);
            return listLibUCBase;
        }

        /// <summary>
        /// Return web link corresponding to parameter enumConversionsType.
        /// </summary>
        /// <param name="enumConversionsType">Enumeration of available conversion types. Samples: EnumConversionsType.Area, EnumConversionsType.Flow, EnumConversionsType.Length.</param>
        /// <returns></returns>
        public static string GetConversionsTypeLink(EnumConversionsType enumConversionsType)
        {
            GetConversionValues(enumConversionsType, out _, out string StringHyperlinkConversion);
            return StringHyperlinkConversion;
        }

        // Use DisplayAttribute in following two methods since they work with UWP apps.
        // More at: https://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.displayattribute
        /// <summary>
        /// Returns string value of EnumConversionsType DisplayAttribute if exists, otherwise return string value of EnumConversionsType.
        /// Sample: Enumeration value '[Display(Description = "Length/Distance")] Length' will return "Length/Distance" versus "Length" since DisplayAttribute exists.
        /// </summary>
        /// <param name="enumConversionsType">Enumeration of available conversion types. Samples: EnumConversionsType.Area, EnumConversionsType.Flow, EnumConversionsType.Length.</param>
        /// <returns></returns>
        public static string GetEnumString(EnumConversionsType enumConversionsType)
        {
            return !(enumConversionsType.GetType()
                .GetField(enumConversionsType.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault() is DisplayAttribute attribute) ? enumConversionsType.ToString() : attribute.Description;
        }

        /// <summary>
        /// Generic overload method that returns string value of enumeration DisplayAttribute if exists, otherwise return string value of enumeration.
        /// This method works with any enumeration.
        /// Sample: Enumeration value '[Display(Description = "Length/Distance")] Length' will return "Length/Distance" versus "Length" since DisplayAttribute exists.
        /// </summary>
        /// <typeparam name="TEnum">Generic enumeration type.</typeparam>
        /// <param name="enumeration">Any enumeration since generic method.</param>
        /// <returns></returns>
        public static string GetEnumString<TEnum>(TEnum enumeration) where TEnum : IComparable, IFormattable, IConvertible
        {
            return !(enumeration.GetType()
                .GetField(enumeration.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault() is DisplayAttribute attribute) ? enumeration.ToString() : attribute.Description;
        }

        /// <summary>
        /// Get libUCBase that contains base unit of parameter enumConversionsType.
        /// </summary>
        /// <param name="enumConversionsType">Enumeration of available conversion types. Samples: EnumConversionsType.Area, EnumConversionsType.Flow, EnumConversionsType.Length.</param>
        /// <returns></returns>
        public static LibUCBase GetConversionsTypeBaseUnit(EnumConversionsType enumConversionsType)
        {
            GetConversionValues(enumConversionsType, out List<LibUCBase> listLibUCBase, out _);
            // Set libUCBase to base unit from retrieved listLibUCBase. Base unit is first item in list and has index of [0].
            LibUCBase libUCBase = new LibUCBase
            (
                listLibUCBase[0].EnumConversions,
                listLibUCBase[0].StringDescription,
                listLibUCBase[0].StringSymbol,
                listLibUCBase[0].DecimalBase,
                listLibUCBase[0].StringHyperlink
            );
            return libUCBase;   // Returned value is base unit of enumConversionsType.
        }

        /// <summary>
        /// Get libUCBase of conversion that matches generic parameter enumConversions.
        /// </summary>
        /// <typeparam name="TEnum">Generic Enum.</typeparam>
        /// <param name="enumConversions">Enum of available conversions. Samples: EnumConversionsArea.acre, EnumConversionsVolume.liter, EnumConversionsLength.foot.</param>
        /// <returns>Unit conversion.</returns>
        public static LibUCBase GetLibUCBase<TEnum>(TEnum enumConversions) where TEnum : IComparable, IFormattable, IConvertible
        {
            // Debug.WriteLine($"LibUC.GetLibUCBase(): Starting method, enumConversions={enumConversions}");
            _ = new List<LibUCBase>();      // List Samples: listConversionsArea, listConversionsFlow, listConversionsLength.
            List<LibUCBase> listLibUCBase = GetListOfConversions(enumConversions);
            // libUCBase needs to be initialized before proceeding. Initialize with base unit of retrieved list. Base unit is first item in list.
            LibUCBase libUCBase = new LibUCBase
            (
                listLibUCBase[0].EnumConversions,
                listLibUCBase[0].StringDescription,
                listLibUCBase[0].StringSymbol,
                listLibUCBase[0].DecimalBase,
                listLibUCBase[0].StringHyperlink
            );
            foreach (LibUCBase libUCBaseFound in listLibUCBase)   // Compare each item in list until match found, then copy and return it.
            {
                if (enumConversions.Equals(libUCBaseFound.EnumConversions))
                {
                    libUCBase = libUCBaseFound;
                    break;
                }
            }
            // Debug.WriteLine($"LibUC.GetLibUCBase(): enumConversions={enumConversions}, Found matching libUCBase={libUCBase}");
            return libUCBase;
        }

        /// <summary>
        /// Get EnumConversionsType from matching string. No parameter checking done.
        /// </summary>
        /// <param name="stringEnumConversionsType">String value of EnumConversionsType. Samples: "Area", "Flow", "Length".</param>
        /// <returns></returns>
        public static EnumConversionsType GetEnumConversionsTypeFromString(string stringEnumConversionsType)
        {
            // Convert stringEnumConversionsType to EnumConversionsType.
            if (!Enum.TryParse(stringEnumConversionsType, out EnumConversionsType enumConversionsType))
            {
                // stringEnumConversionsType not found in enumConversionsType so it may be a DisplayAttribute value.
                // Check cases [Display(Description = "Length/Distance")] Length and [Display(Description = "Solid Angle")] SolidAngle.
                int intMinimumMatchChars = 5;   // Truncate to 5 chars. Result is "Lengt" or "Solid".
                string stringTruncate = stringEnumConversionsType.Remove(intMinimumMatchChars);
                // Debug.WriteLine($"LibUC.GetEnumConversionsTypeFromString(): stringTruncate={stringTruncate}");
                Array arrayConversionsType = Enum.GetValues(typeof(EnumConversionsType));   // Copy EnumConversionsType to array.
                string stringConversionType;
                foreach (object objectFound in arrayConversionsType)
                {
                    stringConversionType = objectFound.ToString();
                    // Debug.WriteLine($"LibUC.GetEnumConversionsTypeFromString(): stringConversionType={stringConversionType}");
                    if (stringConversionType.Contains(stringTruncate))
                    {
                        stringEnumConversionsType = stringConversionType;
                        // Debug.WriteLine($"LibUC.GetEnumConversionsTypeFromString(): Found match! stringEnumConversionsType={stringEnumConversionsType}");
                        break;
                    }
                }
                if (!Enum.TryParse(stringEnumConversionsType, out enumConversionsType))
                {
                    // Throw exception so error can be discovered and corrected.
                    throw new ArgumentOutOfRangeException($"LibUC.GetEnumConversionsTypeFromString(): stringEnumConversionsType={stringEnumConversionsType} not found in EnumConversionsType.");
                }
            }
            return enumConversionsType;     // return enumeration of available conversion types. Samples: EnumConversionsType.Area, EnumConversionsType.Flow, EnumConversionsType.Length.
        }

        /// <summary>
        /// Return temperature converted to Kelvin. To convert temperature, first convert to Kelvin, then convert from Kelvin.
        /// Kelvin is base unit.
        /// </summary>
        /// <param name="decimalValueIn">Temperature value to convert to Kelvin.</param>
        /// <param name="enumConversionsTemperature">Temperature type to convert to Kelvin.</param>
        /// <returns></returns>
        public static decimal ConvertTemperatureToKelvin(decimal decimalValueIn, EnumConversionsTemperature enumConversionsTemperature)
        {
            // Convert decimalValueIn to base unit Kelvin.
            switch (enumConversionsTemperature)
            {
                case EnumConversionsTemperature.kelvin:
                    try
                    {
                        return decimalValueIn;                          // Convert to Kelvin.
                    }
                    catch (OverflowException)
                    {
                        throw;      // Pass exception to calling method to be handled there.
                    }
                case EnumConversionsTemperature.celsius:
                    try
                    {
                        return decimalValueIn + 273.15m;                // Convert to Kelvin.
                    }
                    catch (OverflowException)
                    {
                        throw;      // Pass exception to calling method to be handled there.
                    }
                case EnumConversionsTemperature.fahrenheit:
                    try
                    {
                        return (decimalValueIn + 459.67m) * 5m / 9m;    // Convert to Kelvin.
                    }
                    catch (OverflowException)
                    {
                        throw;      // Pass exception to calling method to be handled there.
                    }
                case EnumConversionsTemperature.rankine:                // Convert to Kelvin.
                    try
                    {
                        return decimalValueIn * 5m / 9m;
                    }
                    catch (OverflowException)
                    {
                        throw;      // Pass exception to calling method to be handled there.
                    }
                default:    // Throw exception so error can be discovered and corrected.
                    throw new NotSupportedException($"LibUC.ConvertTemperatureToKelvin(): Match to enumConversionsTemperature={enumConversionsTemperature} not found in switch statement.");
            }
        }

        /// <summary>
        /// Return temperature converted from Kelvin. To convert temperature, first convert to Kelvin, then convert from Kelvin.
        /// Kelvin is base unit.
        /// </summary>
        /// <param name="decimalValueKelvin">Temperature value to convert from Kelvin.</param>
        /// <param name="enumConversionsTemperature">Temperature type to convert from Kelvin.</param>
        /// <returns></returns>
        public static decimal ConvertTemperatureFromKelvin(decimal decimalValueKelvin, EnumConversionsTemperature enumConversionsTemperature)
        {
            // Convert base unit decimalValueKelvin to output unit.
            switch (enumConversionsTemperature)
            {
                case EnumConversionsTemperature.kelvin:
                    try
                    {
                        return decimalValueKelvin;                          // Convert from Kelvin.
                    }
                    catch (OverflowException)
                    {
                        throw;      // Pass exception to calling method to be handled there.
                    }
                case EnumConversionsTemperature.celsius:
                    try
                    {
                        return decimalValueKelvin - 273.15m;                // Convert from Kelvin.
                    }
                    catch (OverflowException)
                    {
                        throw;      // Pass exception to calling method to be handled there.
                    }
                case EnumConversionsTemperature.fahrenheit:
                    try
                    {
                        return decimalValueKelvin * 9m / 5m - 459.67m;      // Convert from Kelvin.
                    }
                    catch (OverflowException)
                    {
                        throw;      // Pass exception to calling method to be handled there.
                    }
                case EnumConversionsTemperature.rankine:
                    try
                    {
                        return decimalValueKelvin * 9m / 5m;                // Convert from Kelvin.
                    }
                    catch (OverflowException)
                    {
                        throw;      // Pass exception to calling method to be handled there.
                    }
                default:    // Throw exception so error can be discovered and corrected.
                    throw new NotSupportedException($"LibUC.ConvertTemperatureFromKelvin(): Match to enumConversionsTemperature={enumConversionsTemperature} not found in switch statement.");
            }
        }

        /// <summary>
        /// Calculate number of ConversionTypes and total number of unit conversions presently defined in library. 
        /// </summary>
        /// <param name="intNumberConversionTypes">Integer to place number of conversions types presently defined in EnumConversionsType.</param>
        /// <param name="intNumberOfConversions">Integer to place number of unit conversions presently defined in library.</param>
        public static void NumberOfConversions(out int intNumberConversionTypes, out int intNumberOfConversions)
        {
            Array arrayConversionsType = Enum.GetValues(typeof(EnumConversionsType));   // Copy EnumConversionsType to array.
            intNumberConversionTypes = arrayConversionsType.Length;                     // Get number of conversion types in EnumConversionsType.
            int intNumberUnitConversions = 0;
            foreach (object objectFound in arrayConversionsType)
            {
                GetConversionValues((EnumConversionsType)objectFound, out List<LibUCBase> listLibUCBase, out string StringHyperlinkConversion);
                intNumberUnitConversions += listLibUCBase.Count;
            }
            intNumberOfConversions = intNumberUnitConversions;
            // Debug.WriteLine($"LibUC.NumberOfConversions(): intNumberConversionTypes={intNumberConversionTypes}, intNumberOfConversions={intNumberOfConversions}");
        }


        /* Beginning of LIBUC test methods *************************************************************************************/

        /// <summary>
        /// Entry method to test methods in LibUC library are working properly. This method also is sample on how to use methods in library.
        /// </summary>
        public static void TestMethodsMain()
        {
            Debug.WriteLine($"\nLibUC.TestMethodsMain(): Begin testing of LibUC methods");

            Debug.WriteLine($"\nTest EnumConversionsType version of LibUC.GetEnumString():");
            string stringEnumConversionsLength = LibUC.GetEnumString(EnumConversionsType.Length);
            Debug.WriteLine($"LibUC.GetEnumString(EnumConversionsType.Length): stringEnumConversionsLength={stringEnumConversionsLength}");
            string stringEnumConversionsVolume = LibUC.GetEnumString(EnumConversionsType.Volume);
            Debug.WriteLine($"LibUC.GetEnumString(EnumConversionsType.Volume): stringEnumConversionsVolume={stringEnumConversionsVolume}");

            Debug.WriteLine($"\nTest generic overload version of LibUC.GetEnumString():");
            string stringEnumConversionsAcre = LibUC.GetEnumString(EnumConversionsArea.acre);
            Debug.WriteLine($"LibUC.GetEnumString(EnumConversionsArea.acre): stringEnumConversionsAcre={stringEnumConversionsAcre}");
            string stringEnumConversionsDegree = LibUC.GetEnumString(EnumConversionsAngle.degree);
            Debug.WriteLine($"LibUC.GetEnumString(EnumConversionsAngle.degree): stringEnumConversionsDegree={stringEnumConversionsDegree}");

            EnumConversionsType enumConversionsArea = LibUC.GetEnumConversionsTypeFromString("Area");
            Debug.WriteLine($"\nLibUC.GetEnumConversionsTypeFromString(Area): enumConversionsArea={enumConversionsArea}");
            EnumConversionsType enumConversionsSpeed = LibUC.GetEnumConversionsTypeFromString("Speed");
            Debug.WriteLine($"LibUC.GetEnumConversionsTypeFromString(Speed): enumConversionsSpeed={enumConversionsSpeed}");

            LibUCBase libUCBaseAcre = GetLibUCBase(EnumConversionsArea.acre);
            Debug.WriteLine($"\nLibUC.GetLibUCBase(EnumConversionsArea.acre): libUCBaseAcre={libUCBaseAcre}");
            LibUCBase libUCBaseKilogram = GetLibUCBase(EnumConversionsMass.kilogram);
            Debug.WriteLine($"LibUC.GetLibUCBase(EnumConversionsMass.kilogram): libUCBaseKilogram={libUCBaseKilogram}");

            LibUCBase libUCBaseLength = LibUC.GetConversionsTypeBaseUnit(EnumConversionsType.Length);
            Debug.WriteLine($"\nLibUC.GetConversionsTypeBaseUnit(EnumConversionsType.Length): Base unit is libUCBaseLength={libUCBaseLength}");
            LibUCBase libUCBaseVolume = LibUC.GetConversionsTypeBaseUnit(EnumConversionsType.Volume);
            Debug.WriteLine($"LibUC.GetConversionsTypeBaseUnit(EnumConversionsType.Volume): Base unit is libUCBaseVolume={libUCBaseVolume}");

            Debug.WriteLine("");
            // Test method LibUC.GetConversionsTypeList(EnumConversionsType.Area).
            List<LibUCBase> listLibUCArea1 = GetListOfConversions(EnumConversionsArea.acre);
            foreach (LibUCBase libUCBase in listLibUCArea1)
            {
                Debug.WriteLine($"LibUC.GetListOfConversions(EnumConversionsArea.acre): {libUCBase}");
                // Uncomment break to show just first item in libUCBase. Comment out break to show all items in libUCBase.
                break;
            }
            // Test method LibUC.GetListOfConversions(EnumConversionsType.Area).
            List<LibUCBase> listLibUCArea2 = GetListOfConversions(EnumConversionsType.Area);
            foreach (LibUCBase libUCBase in listLibUCArea2)
            {
                Debug.WriteLine($"LibUC.GetConversionsTypeList(EnumConversionsType.Area): {libUCBase}");
                // Uncomment break to show just first item in libUCBase. Comment out break to show all items in libUCBase.
                break;
            }
            Debug.WriteLine("Result of two lines above should be the same!!!");

            // Test temperature conversion methods via roundtrip.
            decimal decimalFahrenheit = 60m;
            decimal decimalKelvin = LibUC.ConvertTemperatureToKelvin(decimalFahrenheit, EnumConversionsTemperature.fahrenheit);
            Debug.WriteLine($"\nLibUC.ConvertTemperatureToKelvin(): Converted decimalFahrenheit={decimalFahrenheit} to decimalKelvin={decimalKelvin}");
            decimal decimalCelcius = LibUC.ConvertTemperatureFromKelvin(decimalKelvin, EnumConversionsTemperature.celsius);
            Debug.WriteLine($"LibUC.ConvertTemperatureFromKelvin(): Converted decimalKelvin={decimalKelvin} to decimalCelcius={decimalCelcius}");
            decimalKelvin = LibUC.ConvertTemperatureToKelvin(decimalCelcius, EnumConversionsTemperature.celsius);
            decimalFahrenheit = LibUC.ConvertTemperatureFromKelvin(decimalKelvin, EnumConversionsTemperature.fahrenheit);
            Debug.WriteLine($"LibUC.ConvertTemperatureFromKelvin(): Converted decimalCelcius={decimalCelcius} to decimalFahrenheit={decimalFahrenheit}");

            // Test ConvertValue() overload methods using various TestMethodsConvertValueXXX() methods.
            Debug.WriteLine("");
            TestMethodsConvertValueDecimal(1m, EnumConversionsLength.mile, EnumConversionsLength.foot);
            TestMethodsConvertValueDouble(1d, EnumConversionsLength.mile, EnumConversionsLength.foot);
            TestMethodsConvertValueFloat(1f, EnumConversionsLength.mile, EnumConversionsLength.foot);
            Debug.WriteLine("");
            // Next line should cause overflow exception to see if working properly.
            TestMethodsConvertValueDecimal(900000000000m, EnumConversionsLength.lightYear, EnumConversionsLength.inch);
            // Test temperature conversion in LibUC.ConvertValue() using TestMethodsConvertValueDecimal().
            TestMethodsConvertValueDecimal(60m, EnumConversionsTemperature.fahrenheit, EnumConversionsTemperature.celsius);

            Debug.WriteLine("");
            NumberOfConversions(out int numConversionTypes, out int numUnitConversions);  // Get tally of conversion types and conversions available.
            Debug.WriteLine($"LibUC.NumberOfConversions(): numConversionTypes={numConversionTypes}, numUnitConversions={numUnitConversions}.");

            Debug.WriteLine($"\nLibUC.TestMethodsMain(): End testing of LibUC methods\n");
        }

        /// <summary>
        /// Test sample that does conversion using a decimal and catches OverflowException.
        /// </summary>
        /// <typeparam name="TEnum">Generic enumeration.</typeparam>
        /// <param name="decimalInput">Decimal value to convert.</param>
        /// <param name="unitIn">Conversion enum input value. Samples: EnumConversionsArea.acre, EnumConversionsVolume.liter, EnumConversionsLength.foot.</param>
        /// <param name="unitOut">Conversion enum output value. Samples same as unitIn.</param>
        private static void TestMethodsConvertValueDecimal<TEnum>(decimal decimalInput, TEnum unitIn, TEnum unitOut) where TEnum : IComparable, IFormattable, IConvertible
        {
            // Debug.WriteLine($"LibUC.TestMethodsConvertValueDecimal(): Starting method, decimalInput={decimalInput}, unitIn={unitIn}, unitOut={unitOut}");
            try
            {
                decimal decimalOutput = ConvertValue(decimalInput, unitIn, unitOut);    // Test decimal overload of ConvertValue().
                Debug.WriteLine($"LibUC.TestMethodsConvertValueDecimal(): Converted decimalInput={decimalInput} from {unitIn} to {unitOut}. Result is decimalOutput={decimalOutput} {unitOut}");
            }
            catch (OverflowException)
            {
                Debug.WriteLine($"LibUC.TestMethodsConvertValueDecimal(): Overflow exception occured. Could not convert decimalInput={decimalInput} from {unitIn} to {unitOut}");
            }
        }

        /// <summary>
        /// Test sample that does conversion using a double and catches OverflowException.
        /// </summary>
        /// <typeparam name="TEnum">Generic enumeration.</typeparam>
        /// <param name="doubleInput">Double value to convert.</param>
        /// <param name="unitIn">Conversion enum input value. Samples: EnumConversionsArea.acre, EnumConversionsVolume.liter, EnumConversionsLength.foot.</param>
        /// <param name="unitOut">Conversion enum output value. Samples same as unitIn.</param>
        private static void TestMethodsConvertValueDouble<TEnum>(double doubleInput, TEnum unitIn, TEnum unitOut) where TEnum : IComparable, IFormattable, IConvertible
        {
            // Debug.WriteLine($"LibUC.TestMethodsConvertValueDouble(): Starting method, doubleInput={doubleInput}, unitIn={unitIn}, unitOut={unitOut}");
            try
            {
                double doubleOutput = ConvertValue(doubleInput, unitIn, unitOut);    // Test double overload of ConvertValue().
                Debug.WriteLine($"LibUC.TestMethodsConvertValueDouble(): Converted doubleInput={doubleInput} from {unitIn} to {unitOut}. Result is doubleOutput={doubleOutput} {unitOut}");
            }
            catch (OverflowException)
            {
                Debug.WriteLine($"LibUC.TestMethodsConvertValueDouble(): Overflow exception occured. Could not convert doubleInput={doubleInput} from {unitIn} to {unitOut}");
            }
        }

        /// <summary>
        /// Test sample that does conversion using a float and catches OverflowException.
        /// </summary>
        /// <typeparam name="TEnum">Generic enumeration.</typeparam>
        /// <param name="floatInput">Float value to convert.</param>
        /// <param name="unitIn">Conversion enum input value. Samples: EnumConversionsArea.acre, EnumConversionsVolume.liter, EnumConversionsLength.foot.</param>
        /// <param name="unitOut">Conversion enum output value. Samples same as unitIn.</param>
        private static void TestMethodsConvertValueFloat<TEnum>(float floatInput, TEnum unitIn, TEnum unitOut) where TEnum : IComparable, IFormattable, IConvertible
        {
            // Debug.WriteLine($"LibUC.TestMethodsConvertValueFloat(): Starting method, floatInput={floatInput}, unitIn={unitIn}, unitOut={unitOut}");
            try
            {
                float floatOutput = ConvertValue(floatInput, unitIn, unitOut);    // Test float overload of ConvertValue().
                Debug.WriteLine($"LibUC.TestMethodsConvertValueFloat(): Converted floatInput={floatInput} from {unitIn} to {unitOut}. Result is floatOutput={floatOutput} {unitOut}");
            }
            catch (OverflowException)
            {
                Debug.WriteLine($"LibUC.TestMethodsConvertValueFloat(): Overflow exception occured. Could not convert floatInput={floatInput} from {unitIn} to {unitOut}");
            }
        }

        /* End of LIBUC test methods - Lists containing available Conversion Types follow **************************************/


        /* Conversions Acceleration ********************************************************************************************/

        #region
        public const string StringHyperlinkAcceleration = "https://en.wikipedia.org/wiki/Acceleration";
        private const string StringHyperlinkAccelerationDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Acceleration";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Acceleration conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsAcceleration = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsAcceleration.meter_per_sec_squared,       "meter/sec²",           "m/s²",     mp_one,                 "https://en.wikipedia.org/wiki/Metre_per_second_squared"),
            new LibUCBase(EnumConversionsAcceleration.decimeter_per_sec_squared,   "decimeter/sec²",       "dm/s²",    mp_deci,                "https://en.wikipedia.org/wiki/Metre_per_second_squared"),
            new LibUCBase(EnumConversionsAcceleration.centimeter_per_sec_squared,  "centimeter/sec²",      "cm/s²",    mp_centi,               "https://en.wikipedia.org/wiki/Metre_per_second_squared"),
            new LibUCBase(EnumConversionsAcceleration.millimeter_per_sec_squared,  "millimeter/sec²",      "mm/s²",    mp_milli,               "https://en.wikipedia.org/wiki/Metre_per_second_squared"),
            new LibUCBase(EnumConversionsAcceleration.decameter_per_sec_squared,   "decameter/sec²",       "dam/s²",   mp_deca,                "https://en.wikipedia.org/wiki/Metre_per_second_squared"),
            new LibUCBase(EnumConversionsAcceleration.hectometer_per_sec_squared,  "hectometer/sec²",      "hm/s²",    mp_hecto,               "https://en.wikipedia.org/wiki/Metre_per_second_squared"),
            new LibUCBase(EnumConversionsAcceleration.kilometer_per_sec_squared,   "kilometer/sec²",       "km/s²",    mp_kilo,                "https://en.wikipedia.org/wiki/Metre_per_second_squared"),
            new LibUCBase(EnumConversionsAcceleration.kilometer_per_min_per_sec,   "kilometer/min/sec",    "km/m/s",   mp_kilo / timeMin,      ""),
            new LibUCBase(EnumConversionsAcceleration.kilometer_per_hour_per_sec,  "kilometer/hour/sec",   "km/h/s",   mp_kilo / timeHour,     ""),
            new LibUCBase(EnumConversionsAcceleration.kilometer_per_day_per_sec,   "kilometer/day/sec",    "km/d/s",   mp_kilo / timeDay,      ""),
            new LibUCBase(EnumConversionsAcceleration.galileo,                     "galileo",              "Gal",      mp_centi,               "https://en.wikipedia.org/wiki/Gal_(unit)"),
            new LibUCBase(EnumConversionsAcceleration.knot_per_sec,                "knot/sec",             "kn/s",     speedKnot,              StringHyperlinkAccelerationDefault),
            new LibUCBase(EnumConversionsAcceleration.inch_per_sec_squared,        "inch/sec²",            "ips²",     lengthInch,             StringHyperlinkAccelerationDefault),
            new LibUCBase(EnumConversionsAcceleration.inch_per_min_per_sec,        "inch/min/sec",         "ipm/s",    lengthInch / timeMin,   StringHyperlinkAccelerationDefault),
            new LibUCBase(EnumConversionsAcceleration.inch_per_hour_per_sec,       "inch/hour/sec",        "iph/s",    lengthInch / timeHour,  ""),
            new LibUCBase(EnumConversionsAcceleration.inch_per_day_per_sec,        "inch/day/sec",         "ipd/s",    lengthInch / timeDay,   ""),
            new LibUCBase(EnumConversionsAcceleration.foot_per_sec_squared,        "foot/sec²",            "fps²",     lengthFoot,             StringHyperlinkAccelerationDefault),
            new LibUCBase(EnumConversionsAcceleration.foot_per_min_per_sec,        "foot/min/sec",         "fpm/s",    lengthFoot / timeMin,   StringHyperlinkAccelerationDefault),
            new LibUCBase(EnumConversionsAcceleration.foot_per_hour_per_sec,       "foot/hour/sec",        "fph/s",    lengthFoot / timeHour,  StringHyperlinkAccelerationDefault),
            new LibUCBase(EnumConversionsAcceleration.foot_per_day_per_sec,        "foot/day/sec",         "fpd/s",    lengthFoot / timeDay,   ""),
            new LibUCBase(EnumConversionsAcceleration.mile_per_sec_squared,        "mile/sec²",            "mps²",     lengthMile,             StringHyperlinkAccelerationDefault),
            new LibUCBase(EnumConversionsAcceleration.mile_per_min_per_sec,        "mile/min/sec",         "mpm/s",    lengthMile / timeMin,   StringHyperlinkAccelerationDefault),
            new LibUCBase(EnumConversionsAcceleration.mile_per_hour_per_sec,       "mile/hour/sec",        "mph/s",    lengthMile / timeHour,  StringHyperlinkAccelerationDefault),
            new LibUCBase(EnumConversionsAcceleration.mile_per_day_per_sec,        "mile/day/sec",         "mpd/s",    lengthMile / timeDay,   ""),
            new LibUCBase(EnumConversionsAcceleration.standardGravity,             "standard-gravity",     "g0",       fpc_g0,                 "https://en.wikipedia.org/wiki/Standard_gravity"),
        };
        #endregion

        /* Conversions Angle ***************************************************************************************************/

        #region
        public const string StringHyperlinkAngle = "https://en.wikipedia.org/wiki/Angle";
        // Not used! private const string StringHyperlinkAngleDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Plane_angle";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Angle conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsAngle = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsAngle.radian,     "radian",     "rad",    mp_one,                     "https://en.wikipedia.org/wiki/Radian"),
            new LibUCBase(EnumConversionsAngle.gradian,    "gradian",    "grad",   fpc_pi / 200m,              "https://en.wikipedia.org/wiki/Gradian"),
            new LibUCBase(EnumConversionsAngle.degree,     "degree",     "˚",      fpc_pi / 180m,              "https://en.wikipedia.org/wiki/Degree_(angle)"),
            new LibUCBase(EnumConversionsAngle.arcminute,  "arcminute",  "'",      fpc_pi / 180m / timeMin,    "https://en.wikipedia.org/wiki/Minute_and_second_of_arc"),
            new LibUCBase(EnumConversionsAngle.arcsecond,  "arcsecond",  "\"",     fpc_pi / 180m / timeHour,   "https://en.wikipedia.org/wiki/Minute_and_second_of_arc"),
            new LibUCBase(EnumConversionsAngle.revolution, "revolution", "",       fpc_pi * 2m,                ""),
            new LibUCBase(EnumConversionsAngle.quadrant,   "quadrant",   "",       fpc_pi / 2m,                "https://en.wikipedia.org/wiki/Circular_sector"),
            new LibUCBase(EnumConversionsAngle.sextant,    "sextant",    "",       fpc_pi / 3m,                "https://en.wikipedia.org/wiki/Circular_sector"),
            new LibUCBase(EnumConversionsAngle.octant,     "octant",     "",       fpc_pi / 4m,                "https://en.wikipedia.org/wiki/Octant_(plane_geometry)"),
            new LibUCBase(EnumConversionsAngle.sign,       "sign",       "",       fpc_pi / 6m,                "https://en.wikipedia.org/wiki/Octant_(plane_geometry)"),
        };
        #endregion

        /* Conversions Area ****************************************************************************************************/

        #region
        public const string StringHyperlinkArea = "https://en.wikipedia.org/wiki/Area";
        private const string StringHyperlinkAreaDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Area";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Area conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsArea = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsArea.meter_squared,       "meter²",       "m²",       mp_one,                         "https://en.wikipedia.org/wiki/Square_metre"),
            new LibUCBase(EnumConversionsArea.centimeter_squared,  "centimeter²",  "cm²",      mp_centi * mp_centi,            "https://en.wikipedia.org/wiki/Square_metre"),
            new LibUCBase(EnumConversionsArea.millimeter_squared,  "millimeter²",  "mm²",      mp_milli * mp_milli,            "https://en.wikipedia.org/wiki/Square_metre"),
            new LibUCBase(EnumConversionsArea.kilometer_squared,   "kilometer²",   "km²",      mp_kilo * mp_kilo,              "https://en.wikipedia.org/wiki/Square_kilometre"),
            new LibUCBase(EnumConversionsArea.are,                 "are",          "a",        areaAre,                        "https://en.wikipedia.org/wiki/Hectare#Are"),
            new LibUCBase(EnumConversionsArea.decare,              "decare",       "daa",      areaAre * mp_deca,              "https://en.wikipedia.org/wiki/Hectare#Decare"),
            new LibUCBase(EnumConversionsArea.hectare,             "hectare",      "ha",       areaAre * mp_hecto,             "https://en.wikipedia.org/wiki/Hectare"),
            new LibUCBase(EnumConversionsArea.dunam,               "dunam",        "",         1000m,                          "https://en.wikipedia.org/wiki/Dunam"),
            new LibUCBase(EnumConversionsArea.stremma,             "stremma",      "",         1000m,                          "https://en.wikipedia.org/wiki/Stremma"),
            new LibUCBase(EnumConversionsArea.inch_squared,        "inch²",        "in²",      areaInchSquared,                "https://en.wikipedia.org/wiki/Square_inch"),
            new LibUCBase(EnumConversionsArea.foot_squared,        "foot²",        "ft²",      areaFootSquared,                "https://en.wikipedia.org/wiki/Square_foot"),
            new LibUCBase(EnumConversionsArea.yard_squared,        "yard²",        "yd²",      areaYardSquared,                "https://en.wikipedia.org/wiki/Square_yard"),
            new LibUCBase(EnumConversionsArea.mile_squared,        "mile²",        "mi²",      areaMileSquared,                "https://en.wikipedia.org/wiki/Square_mile"),
            new LibUCBase(EnumConversionsArea.section,             "section",      "",         areaMileSquared,                "https://en.wikipedia.org/wiki/Section_(United_States_land_surveying)"),
            new LibUCBase(EnumConversionsArea.acre,                "acre",         "ac",       areaMileSquared / 640m,         "https://en.wikipedia.org/wiki/Acre"),
            new LibUCBase(EnumConversionsArea.rood,                "rood",         "ro",       areaMileSquared / 640m / 4m,    StringHyperlinkAreaDefault),
            new LibUCBase(EnumConversionsArea.township,            "township",     "",         areaMileSquared * 36m,          "https://en.wikipedia.org/wiki/Survey_township"),
        };
        #endregion

        /* Conversions AreaMomentFirst *****************************************************************************************/

        #region
        public const string StringHyperlinkAreaMomentFirst = "https://en.wikipedia.org/wiki/First_moment_of_area";
        // Not Used! private const string StringHyperlinkAreaMomentFirstDefault = "";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Area Moment First conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsAreaMomentFirst = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsAreaMomentFirst.meter_cubed,       "meter³",       "m³",       mp_one,                             ""),
            new LibUCBase(EnumConversionsAreaMomentFirst.decimeter_cubed,   "decimeter³",   "dm³",      mp_deci * mp_deci * mp_deci,        ""),
            new LibUCBase(EnumConversionsAreaMomentFirst.centimeter_cubed,  "centimeter³",  "cm³",      volumeCentimeterCubed,              ""),
            new LibUCBase(EnumConversionsAreaMomentFirst.millimeter_cubed,  "millimeter³",  "mm³",      mp_milli * mp_milli * mp_milli,     ""),
            new LibUCBase(EnumConversionsAreaMomentFirst.inch_cubed,        "inch³",        "in³",      volumeInchCubed,                    ""),
            new LibUCBase(EnumConversionsAreaMomentFirst.foot_cubed,        "foot³",        "ft³",      volumeFootCubed,                    ""),
        };
        #endregion

        /* Conversions AreaMomentSecond ****************************************************************************************/

        #region
        public const string StringHyperlinkAreaMomentSecond = "https://en.wikipedia.org/wiki/Second_moment_of_area";
        // Not Used! private const string StringHyperlinkAreaMomentSecondDefault = "";

        // To get exponent values, copy and paste from LibNumerics.

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Area Moment Second conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsAreaMomentSecond = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsAreaMomentSecond.meter_quaded,         "meter⁴",       "m⁴",       mp_one,                                     ""),
            new LibUCBase(EnumConversionsAreaMomentSecond.decimeter_quaded,     "decimeter⁴",   "dm⁴",      mp_deci * mp_deci * mp_deci * mp_deci,      ""),
            new LibUCBase(EnumConversionsAreaMomentSecond.centimeter_quaded,    "centimeter⁴",  "cm⁴",      volumeCentimeterCubed * mp_centi,           ""),
            new LibUCBase(EnumConversionsAreaMomentSecond.millimeter_quaded,    "millimeter⁴",  "mm⁴",      mp_milli * mp_milli * mp_milli * mp_milli,  ""),
            new LibUCBase(EnumConversionsAreaMomentSecond.inch_quaded,          "inch⁴",        "in⁴",      areaInchSquared * areaInchSquared,          ""),
            new LibUCBase(EnumConversionsAreaMomentSecond.foot_quaded,          "foot⁴",        "ft⁴",      areaFootSquared * areaFootSquared,          ""),
        };
        #endregion

        /* Conversions Data ****************************************************************************************************/

        #region
        public const string StringHyperlinkData = "https://en.wikipedia.org/wiki/Binary_prefix";
        // Not Used! private const string StringHyperlinkDataDefault = "";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Data conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsData = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsData.bit,             "bit",          "bit",      mp_one,         "https://en.wikipedia.org/wiki/Bit"),
            new LibUCBase(EnumConversionsData.kilobit,         "kilobit",      "kbit",     mp_kilo,        "https://en.wikipedia.org/wiki/Bit#Multiple_bits"),
            new LibUCBase(EnumConversionsData.megabit,         "megabit",      "Mbit",     mp_mega,        "https://en.wikipedia.org/wiki/Bit#Multiple_bits"),
            new LibUCBase(EnumConversionsData.gigabit,         "gigabit",      "Gbit",     mp_giga,        "https://en.wikipedia.org/wiki/Bit#Multiple_bits"),
            new LibUCBase(EnumConversionsData.terabit,         "terabit",      "Tbit",     mp_tera,        "https://en.wikipedia.org/wiki/Bit#Multiple_bits"),
            new LibUCBase(EnumConversionsData.petabit,         "petabit",      "Pbit",     mp_peta,        "https://en.wikipedia.org/wiki/Bit#Multiple_bits"),
            new LibUCBase(EnumConversionsData.exabit,          "exabit",       "Ebit",     mp_exa,         "https://en.wikipedia.org/wiki/Bit#Multiple_bits"),
            new LibUCBase(EnumConversionsData.zettabit,        "zettabit",     "Zbit",     mp_zetta,       "https://en.wikipedia.org/wiki/Bit#Multiple_bits"),
            new LibUCBase(EnumConversionsData.yottabit,        "yottabit",     "Ybit",     mp_yotta,       "https://en.wikipedia.org/wiki/Bit#Multiple_bits"),
            new LibUCBase(EnumConversionsData.nibble,          "nibble",       "",         4m,             "https://en.wikipedia.org/wiki/Nibble"),
            new LibUCBase(EnumConversionsData.octet,           "octet",        "o",        8m,             "https://en.wikipedia.org/wiki/Octet_(computing)"),
            new LibUCBase(EnumConversionsData.Byte,            "byte",         "B",        8m,             "https://en.wikipedia.org/wiki/Byte"),
            new LibUCBase(EnumConversionsData.kilobyte,        "kilobyte",     "kB",       8m * mp_kilo,   "https://en.wikipedia.org/wiki/Byte#Unit_symbol"),
            new LibUCBase(EnumConversionsData.megabyte,        "megabyte",     "MB",       8m * mp_mega,   "https://en.wikipedia.org/wiki/Byte#Unit_symbol"),
            new LibUCBase(EnumConversionsData.gigabyte,        "gigabyte",     "GB",       8m * mp_giga,   "https://en.wikipedia.org/wiki/Byte#Unit_symbol"),
            new LibUCBase(EnumConversionsData.terabyte,        "terabyte",     "TB",       8m * mp_tera,   "https://en.wikipedia.org/wiki/Byte#Unit_symbol"),
            new LibUCBase(EnumConversionsData.petabyte,        "petabyte",     "PB",       8m * mp_peta,   "https://en.wikipedia.org/wiki/Byte#Unit_symbol"),
            new LibUCBase(EnumConversionsData.exabyte,         "exabyte",      "EB",       8m * mp_exa,    "https://en.wikipedia.org/wiki/Byte#Unit_symbol"),
            new LibUCBase(EnumConversionsData.zettabyte,       "zettabyte",    "ZB",       8m * mp_zetta,  "https://en.wikipedia.org/wiki/Byte#Unit_symbol"),
            new LibUCBase(EnumConversionsData.yottabyte,       "yottabyte",    "YB",       8m * mp_yotta,  "https://en.wikipedia.org/wiki/Byte#Unit_symbol"),
            new LibUCBase(EnumConversionsData.kibibit,         "kibibit",      "Kibit",    1024m,          "https://en.wikipedia.org/wiki/Kibibit"),
            new LibUCBase(EnumConversionsData.mebibit,         "mebibit",      "Mibit",    1024m * 1024m,              "https://en.wikipedia.org/wiki/Mebibit"),
            new LibUCBase(EnumConversionsData.gibibit,         "gibibit",      "Gibit",    1024m * 1024m * 1024m,      "https://en.wikipedia.org/wiki/Gibibit"),
            new LibUCBase(EnumConversionsData.tebibit,         "tebibit",      "Tibit",    1024m * 1024m * 1024m * 1024m,          "https://en.wikipedia.org/wiki/Tebibit"),
            new LibUCBase(EnumConversionsData.pebibit,         "pebibit",      "Pibit",    1024m * 1024m * 1024m * 1024m * 1024m,  "https://en.wikipedia.org/wiki/Tebibit"),
            new LibUCBase(EnumConversionsData.exbibit,         "exbibit",      "Eibit",    1024m * 1024m * 1024m * 1024m * 1024m * 1024m,          "https://en.wikipedia.org/wiki/Exbibit"),
            new LibUCBase(EnumConversionsData.zebibit,         "zebibit",      "Zibit",    1024m * 1024m * 1024m * 1024m * 1024m * 1024m * 1024m,  "https://en.wikipedia.org/wiki/Zebibit"),
            new LibUCBase(EnumConversionsData.yobibit,         "yobibit",      "Yibit",    1024m * 1024m * 1024m * 1024m * 1024m * 1024m * 1024m *1024m,   "https://en.wikipedia.org/wiki/Yobibit"),
            new LibUCBase(EnumConversionsData.kibibyte,        "kibibyte",     "KiB",      8m * 1024m,                 "https://en.wikipedia.org/wiki/Binary_prefix#Adoption_by_IEC.2C_NIST_and_ISO"),
            new LibUCBase(EnumConversionsData.mebibyte,        "mebibyte",     "MiB",      8m * 1024m * 1024m,         "https://en.wikipedia.org/wiki/Binary_prefix#Adoption_by_IEC.2C_NIST_and_ISO"),
            new LibUCBase(EnumConversionsData.gibibyte,        "gibibyte",     "GiB",      8m * 1024m * 1024m * 1024m, "https://en.wikipedia.org/wiki/Binary_prefix#Adoption_by_IEC.2C_NIST_and_ISO"),
            new LibUCBase(EnumConversionsData.tebibyte,        "tebibyte",     "TiB",      8m * 1024m * 1024m * 1024m * 1024m,         "https://en.wikipedia.org/wiki/Binary_prefix#Adoption_by_IEC.2C_NIST_and_ISO"),
            new LibUCBase(EnumConversionsData.pebibyte,        "pebibyte",     "PiB",      8m * 1024m * 1024m * 1024m * 1024m * 1024m, "https://en.wikipedia.org/wiki/Binary_prefix#Adoption_by_IEC.2C_NIST_and_ISO"),
            new LibUCBase(EnumConversionsData.exbibyte,        "exbibyte",     "EiB",      8m * 1024m * 1024m * 1024m * 1024m * 1024m * 1024m,         "https://en.wikipedia.org/wiki/Binary_prefix#Adoption_by_IEC.2C_NIST_and_ISO"),
            new LibUCBase(EnumConversionsData.zebibyte,        "zebibyte",     "ZiB",      8m * 1024m * 1024m * 1024m * 1024m * 1024m * 1024m * 1024m, "https://en.wikipedia.org/wiki/Binary_prefix#Adoption_by_IEC.2C_NIST_and_ISO"),
            new LibUCBase(EnumConversionsData.yobibyte,        "yobibyte",     "YiB",      8m * 1024m * 1024m * 1024m * 1024m * 1024m * 1024m * 1024m *1024m,  "https://en.wikipedia.org/wiki/Binary_prefix#Adoption_by_IEC.2C_NIST_and_ISO"),
        };
        #endregion

        /* Conversions Density *************************************************************************************************/

        #region
        public const string StringHyperlinkDensity = "https://en.wikipedia.org/wiki/Density";
        private const string StringHyperlinkDensityDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Density";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Density conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsDensity = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsDensity.kilogram_per_meter_cubed,     "kilogram/meter³",      "kg/m³",        mp_one,                                         StringHyperlinkDensityDefault),
            new LibUCBase(EnumConversionsDensity.gram_per_meter_cubed,         "gram/meter³",          "g/m³",         massGram,                                       ""),
            new LibUCBase(EnumConversionsDensity.milligram_per_meter_cubed,    "milligram/meter³",     "mg/m³",        massGram * mp_milli,                            ""),
            new LibUCBase(EnumConversionsDensity.gram_per_centimeter_cubed,    "gram/centimeter³",     "g/cm³",        massGram / (mp_centi * mp_centi * mp_centi),    ""),
            new LibUCBase(EnumConversionsDensity.kilogram_per_liter,           "kilogram/liter",       "kg/L",         mp_one / volumeLiter,                           StringHyperlinkDensityDefault),
            new LibUCBase(EnumConversionsDensity.gram_per_milliliter,          "gram/milliliter",      "g/mL",         massGram / (volumeLiter * mp_milli),            StringHyperlinkDensityDefault),
            new LibUCBase(EnumConversionsDensity.ounce_per_foot_cubed,         "ounce/foot³",          "oz/ft³",       massOunce / volumeFootCubed,                    StringHyperlinkDensityDefault),
            new LibUCBase(EnumConversionsDensity.ounce_per_inch_cubed,         "ounce/inch³",          "oz/in³",       massOunce / volumeInchCubed,                    StringHyperlinkDensityDefault),
            new LibUCBase(EnumConversionsDensity.ounce_per_gallonUSfld,        "ounce/gallon-USfld",   "oz/gal USf",   massOunce / volumeGallonUSf,                    StringHyperlinkDensityDefault),
            new LibUCBase(EnumConversionsDensity.ounce_per_gallonIMP,          "ounce/gallon-IMP",     "oz/gal IMP",   massOunce / volumeGallonIMP,                    StringHyperlinkDensityDefault),
            new LibUCBase(EnumConversionsDensity.pound_per_foot_cubed,         "pound/foot³",          "lb/ft³",       massPound / volumeFootCubed,                    StringHyperlinkDensityDefault),
            new LibUCBase(EnumConversionsDensity.pound_per_inch_cubed,         "pound/inch³",          "lb/in³",       massPound / volumeInchCubed,                    StringHyperlinkDensityDefault),
            new LibUCBase(EnumConversionsDensity.pound_per_gallonUSfld,        "pound/gallon-USfld",   "lb/gal USf",   massPound / volumeGallonUSf,                    StringHyperlinkDensityDefault),
            new LibUCBase(EnumConversionsDensity.pound_per_gallonIMP,          "pound/gallon-IMP",     "lb/gal IMP",   massPound / volumeGallonIMP,                    StringHyperlinkDensityDefault),
            new LibUCBase(EnumConversionsDensity.slug_per_foot_cubed,          "slug/foot³",           "slug/ft³",     massSlug / volumeFootCubed,                     StringHyperlinkDensityDefault),
        };
        #endregion

        /* Conversions Energy **************************************************************************************************/

        #region
        public const string StringHyperlinkEnergy = "https://en.wikipedia.org/wiki/Energy";
        private const string StringHyperlinkEnergyDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Energy";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Energy conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsEnergy = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsEnergy.joule,                                     "joule",                        "J",            mp_one,                                     "https://en.wikipedia.org/wiki/Joule"),
            new LibUCBase(EnumConversionsEnergy.coulomb_volt,                              "coulomb·volt",                 "C·V",          mp_one,                                     "https://en.wikipedia.org/wiki/Joule"),
            new LibUCBase(EnumConversionsEnergy.kilogram_meter_squared_per_sec_squared,    "kilogram·meter²/sec²",         "kg·m²/s²",     mp_one,                                     "https://en.wikipedia.org/wiki/Joule"),
            new LibUCBase(EnumConversionsEnergy.newton_meter,                              "newton·meter",                 "N·m",          mp_one,                                     "https://en.wikipedia.org/wiki/Newton_metre"),
            new LibUCBase(EnumConversionsEnergy.pascal_meter_cubed,                        "pascal·meter³",                "Pa·m³",        mp_one,                                     "https://en.wikipedia.org/wiki/Joule"),
            new LibUCBase(EnumConversionsEnergy.watt_sec,                                  "watt·sec",                     "W·s",          mp_one,                                     "https://en.wikipedia.org/wiki/Joule"),
            new LibUCBase(EnumConversionsEnergy.millijoule,                                "millijoule",                   "mJ",           mp_milli,                                   "https://en.wikipedia.org/wiki/Joule#Millijoule"),
            new LibUCBase(EnumConversionsEnergy.microjoule,                                "microjoule",                   "μJ",           mp_micro,                                   "https://en.wikipedia.org/wiki/Joule#Microjoule"),
            new LibUCBase(EnumConversionsEnergy.nanojoule,                                 "nanojoule",                    "nJ",           mp_nano,                                    "https://en.wikipedia.org/wiki/Joule#Nanojoule"),
            new LibUCBase(EnumConversionsEnergy.picojoule,                                 "picojoule",                    "pJ",           mp_pico,                                    "https://en.wikipedia.org/wiki/Joule"),
            new LibUCBase(EnumConversionsEnergy.kilojoule,                                 "kilojoule",                    "kJ",           mp_kilo,                                    "https://en.wikipedia.org/wiki/Joule#Kilojoule"),
            new LibUCBase(EnumConversionsEnergy.megajoule,                                 "megajoule",                    "MJ",           mp_mega,                                    "https://en.wikipedia.org/wiki/Joule#Megajoule"),
            new LibUCBase(EnumConversionsEnergy.gigajoule,                                 "gigajoule",                    "GJ",           mp_giga,                                    "https://en.wikipedia.org/wiki/Joule#Gigajoule"),
            new LibUCBase(EnumConversionsEnergy.terajoule,                                 "terajoule",                    "TJ",           mp_tera,                                    "https://en.wikipedia.org/wiki/Joule#Terajoule"),
            new LibUCBase(EnumConversionsEnergy.erg,                                       "erg",                          "erg",          0.0000001m,                                 "https://en.wikipedia.org/wiki/Erg"),
            new LibUCBase(EnumConversionsEnergy.celsiusHeatUnitIT,                         "celsius-heat-unit-IT",         "CHU-IT",       energyBtuIT * 1.8m,                         StringHyperlinkEnergyDefault),
            new LibUCBase(EnumConversionsEnergy.dyne_centimeter,                           "dyne·centimeter",              "dyn·cm",       forceDyne * mp_centi,                       ""),
            new LibUCBase(EnumConversionsEnergy.watt_hour,                                 "watt·hour",                    "W·h",          timeHour,                                   "https://en.wikipedia.org/wiki/Kilowatt_hour"),
            new LibUCBase(EnumConversionsEnergy.milliwatt_hour,                            "milliwatt·hour",               "mW·h",         timeHour * mp_milli,                        "https://en.wikipedia.org/wiki/Kilowatt_hour"),
            new LibUCBase(EnumConversionsEnergy.microwatt_hour,                            "microwatt·hour",               "μW·h",         timeHour * mp_micro,                        "https://en.wikipedia.org/wiki/Kilowatt_hour"),
            new LibUCBase(EnumConversionsEnergy.kilowatt_hour,                             "kilowatt·hour",                "kW·h",         timeHour * mp_kilo,                         "https://en.wikipedia.org/wiki/Kilowatt_hour"),
            new LibUCBase(EnumConversionsEnergy.megawatt_hour,                             "megawatt·hour",                "MW·h",         timeHour * mp_mega,                         "https://en.wikipedia.org/wiki/Kilowatt_hour"),
            new LibUCBase(EnumConversionsEnergy.gigawatt_hour,                             "gigawatt·hour",                "GW·h",         timeHour * mp_giga,                         "https://en.wikipedia.org/wiki/Kilowatt_hour"),
            new LibUCBase(EnumConversionsEnergy.terawatt_hour,                             "terawatt·hour",                "TW·h",         timeHour * mp_tera,                         "https://en.wikipedia.org/wiki/Kilowatt_hour"),
            new LibUCBase(EnumConversionsEnergy.petawatt_hour,                             "petawatt·hour",                "PW·h",         timeHour * mp_peta,                         "https://en.wikipedia.org/wiki/Kilowatt_hour"),
            new LibUCBase(EnumConversionsEnergy.kilogramForce_meter,                       "kilogram-force·meter",         "kgf·m",        forceKilogram,                              "https://en.wikipedia.org/wiki/Kilogram-force"),
            new LibUCBase(EnumConversionsEnergy.kilogramForce_centimeter,                  "kilogram-force·centimeter",    "kgf·cm",       forceKilogram * mp_centi,                   "https://en.wikipedia.org/wiki/Kilogram-force"),
            new LibUCBase(EnumConversionsEnergy.gramForce_centimeter,                      "gram-force·centimeter",        "gf·cm",        forceKilogram * massGram * mp_centi,        ""),
            new LibUCBase(EnumConversionsEnergy.ounceForce_inch,                           "ounce-force·inch",             "ozf·in",       forceOunce * lengthInch,                    ""),
            new LibUCBase(EnumConversionsEnergy.ounceForce_foot,                           "ounce-force·foot",             "ozf·ft",       forceOunce * lengthFoot,                    ""),
            new LibUCBase(EnumConversionsEnergy.poundForce_inch,                           "pound-force·inch",             "lbf·in",       forcePound * lengthInch,                    "https://en.wikipedia.org/wiki/Foot-pound_(energy)"),
            new LibUCBase(EnumConversionsEnergy.poundForce_foot,                           "pound-force·foot",             "lbf·ft",       forcePound * lengthFoot,                    "https://en.wikipedia.org/wiki/Foot-pound_(energy)"),
            new LibUCBase(EnumConversionsEnergy.poundal_inch,                              "poundal·inch",                 "pdl·in",       forcePoundal * lengthInch,                  ""),
            new LibUCBase(EnumConversionsEnergy.poundal_foot,                              "poundal·foot",                 "pdl·ft",       forcePoundal * lengthFoot,                  ""),
            new LibUCBase(EnumConversionsEnergy.horsepowerBoiler_hour,                     "horsepower-boiler·hour",       "hp·hr",        powerHorsepowerBoiler * timeHour,           ""),
            new LibUCBase(EnumConversionsEnergy.horsepowerElectric_hour,                   "horsepower-electric·hour",     "hp·hr",        powerHorsepowerElectric * timeHour,         ""),
            new LibUCBase(EnumConversionsEnergy.horsepowerMechanical_hour,                 "horsepower-mechanical·hour",   "hp·hr",        powerHorsepowerMechanical * timeHour,       StringHyperlinkEnergyDefault),
            new LibUCBase(EnumConversionsEnergy.horsepowerMetric_hour,                     "horsepower-metric·hour",       "hp·hr",        powerHorsepowerMetric * timeHour,           ""),
            new LibUCBase(EnumConversionsEnergy.tonCoalEquiv,                              "ton-coal-equivalent",          "TCE",          energyCalorieTH * mp_giga * 7m,             StringHyperlinkEnergyDefault),
            new LibUCBase(EnumConversionsEnergy.tonOilEquiv,                               "ton-oil-equivalent",           "toe",          energyCalorieIT * mp_giga * 10m,            "https://en.wikipedia.org/wiki/Tonne_of_oil_equivalent"),
            new LibUCBase(EnumConversionsEnergy.tonTNTEquiv,                               "ton-TNT-equivalent",           "tTNT",         energyCalorieTH * mp_giga,                  StringHyperlinkEnergyDefault),
            new LibUCBase(EnumConversionsEnergy.atmosphere_liter,                          "atmosphere·liter",             "atm·L",        pressureAtmosphere * volumeLiter,           StringHyperlinkEnergyDefault),
            new LibUCBase(EnumConversionsEnergy.atmosphere_meter_cubed,                    "atmosphere·meter³",            "atm·m³",       pressureAtmosphere,                         ""),
            new LibUCBase(EnumConversionsEnergy.atmosphere_centimeter_cubed,               "atmosphere·centimeter³",       "atm·cm³",      pressureAtmosphere * volumeCentimeterCubed, StringHyperlinkEnergyDefault),
            new LibUCBase(EnumConversionsEnergy.atmosphere_inch_cubed,                     "atmosphere·inch³",             "atm·in³",      pressureAtmosphere * volumeInchCubed,       ""),
            new LibUCBase(EnumConversionsEnergy.atmosphere_foot_cubed,                     "atmosphere·foot³",             "atm·ft³",      pressureAtmosphere * volumeFootCubed,       StringHyperlinkEnergyDefault),
            new LibUCBase(EnumConversionsEnergy.calorieC15,                                "calorie-15°C",                 "cal-15C",      energyCalorieC15,                           "https://en.wikipedia.org/wiki/Calorie"),
            new LibUCBase(EnumConversionsEnergy.calorieFood,                               "calorie-Food",                 "Cal-Food",     energyCalorieTH * mp_kilo,                  "https://en.wikipedia.org/wiki/Calorie"),
            new LibUCBase(EnumConversionsEnergy.calorieIT,                                 "calorie-IT",                   "cal-IT",       energyCalorieIT,                            "https://en.wikipedia.org/wiki/Calorie"),
            new LibUCBase(EnumConversionsEnergy.calorieTH,                                 "calorie-TH",                   "cal-TH",       energyCalorieTH,                            "https://en.wikipedia.org/wiki/Calorie"),
            new LibUCBase(EnumConversionsEnergy.kilocalorieC15,                            "kilocalorie-15°C",             "kcal-15C",     energyCalorieC15 * mp_kilo,                 "https://en.wikipedia.org/wiki/Calorie"),
            new LibUCBase(EnumConversionsEnergy.kilocalorieFood,                           "kilocalorie-Food",             "kcal-Food",    energyCalorieTH * mp_kilo,                  "https://en.wikipedia.org/wiki/Calorie"),
            new LibUCBase(EnumConversionsEnergy.kilocalorieIT,                             "kilocalorie-IT",               "kcal-IT",      energyCalorieIT * mp_kilo,                  "https://en.wikipedia.org/wiki/Calorie"),
            new LibUCBase(EnumConversionsEnergy.kilocalorieTH,                             "kilocalorie-TH",               "kcal-TH",      energyCalorieTH * mp_kilo,                  "https://en.wikipedia.org/wiki/Calorie"),
            new LibUCBase(EnumConversionsEnergy.thermieIT,                                 "thermie-IT",                   "th-IT",        energyCalorieIT * mp_mega,                  "https://en.wikipedia.org/wiki/Therm"),
            new LibUCBase(EnumConversionsEnergy.btuC15,                                    "btu-15°C",                     "BTU-15C",      energyBtuC15,                               "http://www.wikidoc.org/index.php/British_thermal_unit"),
            new LibUCBase(EnumConversionsEnergy.btuIMP,                                    "btu-IMP",                      "BTU-IMP",      energyBtuIMP,                               "https://en.wikipedia.org/wiki/British_thermal_unit"),
            new LibUCBase(EnumConversionsEnergy.btuISO,                                    "btu-ISO",                      "BTU-ISO",      energyBtuISO,                               "http://www.wikidoc.org/index.php/British_thermal_unit"),
            new LibUCBase(EnumConversionsEnergy.btuIT,                                     "btu-IT",                       "BTU-IT",       energyBtuIT,                                "http://www.wikidoc.org/index.php/British_thermal_unit"),
            new LibUCBase(EnumConversionsEnergy.btuTH,                                     "btu-TH",                       "BTU-TH",       energyBtuTH,                                "http://www.wikidoc.org/index.php/British_thermal_unit"),
            new LibUCBase(EnumConversionsEnergy.thermC15,                                  "therm-15°C",                   "thm-15C",      energyBtuC15 * 100000m,                     "https://en.wikipedia.org/wiki/Therm"),
            new LibUCBase(EnumConversionsEnergy.thermIMP,                                  "therm-IMP",                    "thm-IMP",      energyBtuIMP * 100000m,                     "https://en.wikipedia.org/wiki/Therm"),
            new LibUCBase(EnumConversionsEnergy.thermISO,                                  "therm-ISO",                    "thm-ISO",      energyBtuISO * 100000m,                     "https://en.wikipedia.org/wiki/Therm"),
            new LibUCBase(EnumConversionsEnergy.thermIT,                                   "therm-IT",                     "thm-IT",       energyBtuIT * 100000m,                      "https://en.wikipedia.org/wiki/Therm"),
            new LibUCBase(EnumConversionsEnergy.thermTH,                                   "therm-TH",                     "thm-TH",       energyBtuTH * 100000m,                      "https://en.wikipedia.org/wiki/Therm"),
            new LibUCBase(EnumConversionsEnergy.quad,                                      "quad",                         "",             energyBtuIT * 1E15m,                        StringHyperlinkEnergyDefault),
            new LibUCBase(EnumConversionsEnergy.atomicMassUnit,                            "atomic-mass-unit",             "u",            fpc_u,                                      ""),
            new LibUCBase(EnumConversionsEnergy.electronvolt,                              "electronvolt",                 "eV",           fpc_eV,                                     "https://en.wikipedia.org/wiki/Electronvolt"),
            new LibUCBase(EnumConversionsEnergy.hartree,                                   "hartree",                      "Ha",           fpc_Ha,                                     "https://en.wikipedia.org/wiki/Hartree"),
            new LibUCBase(EnumConversionsEnergy.kelvin,                                    "kelvin",                       "K",            energyKelvin,                               ""),
            new LibUCBase(EnumConversionsEnergy.kilogram,                                  "kilogram",                     "kg",           fpc_c * fpc_c,                              ""),
        };

        #endregion

        /* Conversions Flow ****************************************************************************************************/

        #region
        public const string StringHyperlinkFlow = "";
        // Not used! private const string StringHyperlinkFlowDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Flow_.28volume.29";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Flow conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsFlow = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsFlow.meter_cubed_per_sec,         "meter³/sec",                   "m³/s",         mp_one,                                         "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsFlow.meter_cubed_per_min,         "meter³/min",                   "m³/m",         mp_one / timeMin,                               "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsFlow.meter_cubed_per_hour,        "meter³/hour",                  "m³/h",         mp_one / timeHour,                              "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsFlow.meter_cubed_per_day,         "meter³/day",                   "m³/d",         mp_one / timeDay,                               "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsFlow.decimeter_cubed_per_sec,     "decimeter³/sec",               "dm³/s",        mp_deci * mp_deci * mp_deci,                    "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsFlow.decimeter_cubed_per_min,     "decimeter³/min",               "dm³/m",        mp_deci * mp_deci * mp_deci / timeMin,          "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsFlow.decimeter_cubed_per_hour,    "decimeter³/hour",              "dm³/h",        mp_deci * mp_deci * mp_deci / timeHour,         "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsFlow.decimeter_cubed_per_day,     "decimeter³/day",               "dm³/d",        mp_deci * mp_deci * mp_deci / timeDay,          "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsFlow.centimeter_cubed_per_sec,    "centimeter³/sec",              "cm³/s",        mp_centi * mp_centi * mp_centi,                 "https://en.wikipedia.org/wiki/Cubic_centimetre"),
            new LibUCBase(EnumConversionsFlow.centimeter_cubed_per_min,    "centimeter³/min",              "cm³/m",        mp_centi * mp_centi * mp_centi / timeMin,       "https://en.wikipedia.org/wiki/Cubic_centimetre"),
            new LibUCBase(EnumConversionsFlow.centimeter_cubed_per_hour,   "centimeter³/hour",             "cm³/h",        mp_centi * mp_centi * mp_centi / timeHour,      "https://en.wikipedia.org/wiki/Cubic_centimetre"),
            new LibUCBase(EnumConversionsFlow.centimeter_cubed_per_day,    "centimeter³/day",              "cm³/d",        mp_centi * mp_centi * mp_centi / timeDay,       "https://en.wikipedia.org/wiki/Cubic_centimetre"),
            new LibUCBase(EnumConversionsFlow.millimeter_cubed_per_sec,    "millimeter³/sec",              "mm³/s",        mp_milli * mp_milli * mp_milli,                 "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsFlow.millimeter_cubed_per_min,    "millimeter³/min",              "mm³/m",        mp_milli * mp_milli * mp_milli / timeMin,       "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsFlow.millimeter_cubed_per_hour,   "millimeter³/hour",             "mm³/h",        mp_milli * mp_milli * mp_milli / timeHour,      "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsFlow.millimeter_cubed_per_day,    "millimeter³/day",              "mm³/d",        mp_milli * mp_milli * mp_milli / timeDay,       "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsFlow.liter_per_sec,               "liter/sec",                    "L/s",          volumeLiter,                                    "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsFlow.liter_per_min,               "liter/min",                    "L/m",          volumeLiter / timeMin,                          "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsFlow.liter_per_hour,              "liter/hour",                   "L/h",          volumeLiter / timeHour,                         "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsFlow.liter_per_day,               "liter/day",                    "L/d",          volumeLiter / timeDay,                          "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsFlow.milliliter_per_sec,          "milliliter/sec",               "mL/s",         volumeLiter * mp_milli,                         "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsFlow.milliliter_per_min,          "milliliter/min",               "mL/m",         volumeLiter * mp_milli / timeMin,               "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsFlow.milliliter_per_hour,         "milliliter/hour",              "mL/h",         volumeLiter * mp_milli / timeHour,              "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsFlow.milliliter_per_day,          "milliliter/day",               "mL/d",         volumeLiter * mp_milli / timeDay,               "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsFlow.microliter_per_sec,          "microliter/sec",               "μL/s",         volumeLiter * mp_micro,                         "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsFlow.microliter_per_min,          "microliter/min",               "μL/m",         volumeLiter * mp_micro / timeMin,               "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsFlow.microliter_per_hour,         "microliter/hour",              "μL/h",         volumeLiter * mp_micro / timeHour,              "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsFlow.microliter_per_day,          "microliter/day",               "μL/d",         volumeLiter * mp_micro / timeDay,               "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsFlow.inch_cubed_per_sec,          "inch³/sec",                    "in³/s",        volumeInchCubed,                                "https://en.wikipedia.org/wiki/Cubic_inch"),
            new LibUCBase(EnumConversionsFlow.inch_cubed_per_min,          "inch³/min",                    "in³/m",        volumeInchCubed / timeMin,                      "https://en.wikipedia.org/wiki/Cubic_inch"),
            new LibUCBase(EnumConversionsFlow.inch_cubed_per_hour,         "inch³/hour",                   "in³/h",        volumeInchCubed / timeHour,                     "https://en.wikipedia.org/wiki/Cubic_inch"),
            new LibUCBase(EnumConversionsFlow.inch_cubed_per_day,          "inch³/day",                    "in³/d",        volumeInchCubed / timeDay,                      "https://en.wikipedia.org/wiki/Cubic_inch"),
            new LibUCBase(EnumConversionsFlow.foot_cubed_per_sec,          "foot³/sec",                    "ft³/s",        volumeFootCubed,                                "https://en.wikipedia.org/wiki/Cubic_foot"),
            new LibUCBase(EnumConversionsFlow.foot_cubed_per_min,          "foot³/min",                    "ft³/m",        volumeFootCubed / timeMin,                      "https://en.wikipedia.org/wiki/Cubic_foot"),
            new LibUCBase(EnumConversionsFlow.foot_cubed_per_hour,         "foot³/hour",                   "ft³/h",        volumeFootCubed / timeHour,                     "https://en.wikipedia.org/wiki/Cubic_foot"),
            new LibUCBase(EnumConversionsFlow.foot_cubed_per_day,          "foot³/day",                    "ft³/d",        volumeFootCubed / timeDay,                      "https://en.wikipedia.org/wiki/Cubic_foot"),
            new LibUCBase(EnumConversionsFlow.yard_cubed_per_sec,          "yard³/sec",                    "yd³/s",        volumeYardCubed,                                "https://en.wikipedia.org/wiki/Cubic_yard"),
            new LibUCBase(EnumConversionsFlow.yard_cubed_per_min,          "yard³/min",                    "yd³/m",        volumeYardCubed / timeMin,                      "https://en.wikipedia.org/wiki/Cubic_yard"),
            new LibUCBase(EnumConversionsFlow.yard_cubed_per_hour,         "yard³/hour",                   "yd³/h",        volumeYardCubed / timeHour,                     "https://en.wikipedia.org/wiki/Cubic_yard"),
            new LibUCBase(EnumConversionsFlow.yard_cubed_per_day,          "yard³/day",                    "yd³/d",        volumeYardCubed / timeDay,                      "https://en.wikipedia.org/wiki/Cubic_yard"),
            new LibUCBase(EnumConversionsFlow.acre_foot_per_sec,           "acre·foot/sec",                "ac·ft/s",      areaMileSquared / 640m * lengthFoot,            "https://en.wikipedia.org/wiki/Acre-foot"),
            new LibUCBase(EnumConversionsFlow.acre_foot_per_min,           "acre·foot/min",                "ac·ft/m",      areaMileSquared / 640m * lengthFoot / timeMin,  "https://en.wikipedia.org/wiki/Acre-foot"),
            new LibUCBase(EnumConversionsFlow.acre_foot_per_hour,          "acre·foot/hour",               "ac·ft/h",      areaMileSquared / 640m * lengthFoot / timeHour, "https://en.wikipedia.org/wiki/Acre-foot"),
            new LibUCBase(EnumConversionsFlow.acre_foot_per_day,           "acre·foot/day",                "ac·ft/d",      areaMileSquared / 640m * lengthFoot / timeDay,  "https://en.wikipedia.org/wiki/Acre-foot"),
            new LibUCBase(EnumConversionsFlow.gallonUSfldWine_per_sec,     "gallon-USfld-Wine/sec",        "gal USf/s",    volumeGallonUSf,                                "https://en.wikipedia.org/wiki/Gallon"),
            new LibUCBase(EnumConversionsFlow.gallonUSfldWine_per_min,     "gallon-USfld-Wine/min",        "gal USf/m",    volumeGallonUSf / timeMin,                      "https://en.wikipedia.org/wiki/Gallon"),
            new LibUCBase(EnumConversionsFlow.gallonUSfldWine_per_hour,    "gallon-USfld-Wine/hour",       "gal USf/h",    volumeGallonUSf / timeHour,                     "https://en.wikipedia.org/wiki/Gallon"),
            new LibUCBase(EnumConversionsFlow.gallonUSfldWine_per_day,     "gallon-USfld-Wine/day",        "gal USf/d",    volumeGallonUSf / timeDay,                   "https://en.wikipedia.org/wiki/Gallon"),
            new LibUCBase(EnumConversionsFlow.ounceUSfld_per_sec,          "ounce-USfld/sec",              "oz USf/s",     volumeGallonUSf / 128m,                         "https://en.wikipedia.org/wiki/Fluid_ounce"),
            new LibUCBase(EnumConversionsFlow.ounceUSfld_per_min,          "ounce-USfld/min",              "oz USf/m",     volumeGallonUSf / 128m / timeMin,               "https://en.wikipedia.org/wiki/Fluid_ounce"),
            new LibUCBase(EnumConversionsFlow.ounceUSfld_per_hour,         "ounce-USfld/hour",             "oz USf/h",     volumeGallonUSf / 128m / timeHour,              "https://en.wikipedia.org/wiki/Fluid_ounce"),
            new LibUCBase(EnumConversionsFlow.ounceUSfld_per_day,          "ounce-USfld/day",              "oz USf/d",     volumeGallonUSf / 128m / timeDay,               "https://en.wikipedia.org/wiki/Fluid_ounce"),
            new LibUCBase(EnumConversionsFlow.barrelUSfldPetro_per_sec,    "barrel-USfld-Petro/sec",       "bbl USf/s",    volumeGallonUSf * 42m,                          "https://en.wikipedia.org/wiki/Barrel_(unit)"),
            new LibUCBase(EnumConversionsFlow.barrelUSfldPetro_per_min,    "barrel-USfld-Petro/min",       "bbl USf/m",    volumeGallonUSf * 42m / timeMin,                "https://en.wikipedia.org/wiki/Barrel_(unit)"),
            new LibUCBase(EnumConversionsFlow.barrelUSfldPetro_per_hour,   "barrel-USfld-Petro/hour",      "bbl USf/h",    volumeGallonUSf * 42m / timeHour,               "https://en.wikipedia.org/wiki/Barrel_(unit)"),
            new LibUCBase(EnumConversionsFlow.barrelUSfldPetro_per_day,    "barrel-USfld-Petro/day",       "bbl USf/d",    volumeGallonUSf * 42m / timeDay,                "https://en.wikipedia.org/wiki/Barrel_(unit)"),
            new LibUCBase(EnumConversionsFlow.gallonIMP_per_sec,           "gallon-IMP/sec",               "gal IMP/s",    volumeGallonIMP,                                "https://en.wikipedia.org/wiki/Gallon"),
            new LibUCBase(EnumConversionsFlow.gallonIMP_per_min,           "gallon-IMP/min",               "gal IMP/m",    volumeGallonIMP / timeMin,                      "https://en.wikipedia.org/wiki/Gallon"),
            new LibUCBase(EnumConversionsFlow.gallonIMP_per_hour,          "gallon-IMP/hour",              "gal IMP/h",    volumeGallonIMP / timeHour,                     "https://en.wikipedia.org/wiki/Gallon"),
            new LibUCBase(EnumConversionsFlow.gallonIMP_per_day,           "gallon-IMP/day",               "gal IMP/d",    volumeGallonIMP / timeDay,                      "https://en.wikipedia.org/wiki/Gallon"),
            new LibUCBase(EnumConversionsFlow.ounceIMP_per_sec,            "ounce-IMP/sec",                "oz IMP/s",     volumeGallonIMP / 160m,                         "https://en.wikipedia.org/wiki/Fluid_ounce"),
            new LibUCBase(EnumConversionsFlow.ounceIMP_per_min,            "ounce-IMP/min",                "oz IMP/m",     volumeGallonIMP / 160m / timeMin,               "https://en.wikipedia.org/wiki/Fluid_ounce"),
            new LibUCBase(EnumConversionsFlow.ounceIMP_per_hour,           "ounce-IMP/hour",               "oz IMP/h",     volumeGallonIMP / 160m /timeHour,               "https://en.wikipedia.org/wiki/Fluid_ounce"),
            new LibUCBase(EnumConversionsFlow.ounceIMP_per_day,            "ounce-IMP/day",                "oz IMP/d",     volumeGallonIMP / 160m / timeDay,               "https://en.wikipedia.org/wiki/Fluid_ounce"),
            new LibUCBase(EnumConversionsFlow.barrelIMP_per_sec,           "barrel-IMP/sec",               "bl IMP/s",     volumeGallonIMP * 36m,                          "https://en.wikipedia.org/wiki/Barrel_(unit)"),
            new LibUCBase(EnumConversionsFlow.barrelIMP_per_min,           "barrel-IMP/min",               "bl IMP/m",     volumeGallonIMP * 36m /timeMin,                 "https://en.wikipedia.org/wiki/Barrel_(unit)"),
            new LibUCBase(EnumConversionsFlow.barrelIMP_per_hour,          "barrel-IMP/hour",              "bl IMP/h",     volumeGallonIMP * 36m / timeHour,               "https://en.wikipedia.org/wiki/Barrel_(unit)"),
            new LibUCBase(EnumConversionsFlow.barrelIMP_per_day,           "barrel-IMP/day",               "bl IMP/d",     volumeGallonIMP * 36m /timeDay,                 "https://en.wikipedia.org/wiki/Barrel_(unit)"),
            
        };
        #endregion

        /* Conversions Force ***************************************************************************************************/

        #region
        public const string StringHyperlinkForce = "https://en.wikipedia.org/wiki/Force";
        private const string StringHyperlinkForceDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Force";

        // one, deci, centi, milli, micro, nano, pico, femto, atto, zepto, yocto, deca, hecto, kilo, mega, giga, tera, peta, exa, zetta, yotta.
        // Set base unit to first item in list.
        /// <summary>
        /// List of available Force conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsForce = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsForce.newton,                         "newton",               "N",            mp_one,                     "https://en.wikipedia.org/wiki/Newton_(unit)"),
            new LibUCBase(EnumConversionsForce.kilogram_meter_per_sec_squared, "kilogram·meter/sec²",  "kg·m/s²",      mp_one,                     "https://en.wikipedia.org/wiki/Newton_(unit)"),
            new LibUCBase(EnumConversionsForce.millinewton,                    "millinewton",          "mN",           mp_milli,                   "https://en.wikipedia.org/wiki/Newton_(unit)"),
            new LibUCBase(EnumConversionsForce.micronewton,                    "micronewton",          "µN",           mp_micro,                   "https://en.wikipedia.org/wiki/Newton_(unit)"),
            new LibUCBase(EnumConversionsForce.kilonewton,                     "kilonewton",           "kN",           mp_kilo,                    "https://en.wikipedia.org/wiki/Newton_(unit)#Commonly_seen_as_kilonewtons"),
            new LibUCBase(EnumConversionsForce.meganewton,                     "meganewton",           "MN",           mp_mega,                    "https://en.wikipedia.org/wiki/Newton_(unit)"),
            new LibUCBase(EnumConversionsForce.dyne,                           "dyne",                 "dyn",          forceDyne,                  "https://en.wikipedia.org/wiki/Dyne"),
            new LibUCBase(EnumConversionsForce.kilogramForce,                  "kilogram-force",       "kgf",          forceKilogram,              "https://en.wikipedia.org/wiki/Kilogram-force"),
            new LibUCBase(EnumConversionsForce.gramForce,                      "gram-force",           "gf",           forceKilogram * massGram,   "https://en.wikipedia.org/wiki/Kilogram-force"),
            new LibUCBase(EnumConversionsForce.ounceForce,                     "ounce-force",          "ozf",          forceOunce,                 "https://en.wikipedia.org/wiki/Pound_(force)"),
            new LibUCBase(EnumConversionsForce.poundForce,                     "pound-force",          "lbf",          forcePound,                 "https://en.wikipedia.org/wiki/Pound_(force)"),
            new LibUCBase(EnumConversionsForce.kipForce,                       "kip-force",            "kipf",         forcePound * 1000m,         "https://en.wikipedia.org/wiki/Kip_(unit)"),
            new LibUCBase(EnumConversionsForce.tonForceUS,                     "ton-force-US",         "tnf short",    forcePound * 2000m,         StringHyperlinkForceDefault),
            new LibUCBase(EnumConversionsForce.tonForceIMP,                    "ton-force-IMP",        "tnf long",     forcePound * 2240m,         StringHyperlinkForceDefault),
            new LibUCBase(EnumConversionsForce.poundal,                        "poundal",              "pdl",          forcePoundal,               "https://en.wikipedia.org/wiki/Poundal"),
        };
        #endregion

        /* Conversions Frequency ***********************************************************************************************/

        #region
        public const string StringHyperlinkFrequency = "https://en.wikipedia.org/wiki/Frequency";
        // Not used! private const string StringHyperlinkFrequencyDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Frequency";

        // one, deci, centi, milli, micro, nano, pico, femto, atto, zepto, yocto, deca, hecto, kilo, mega, giga, tera, peta, exa, zetta, yotta.
        // Set base unit to first item in list.
        /// <summary>
        /// List of available Force conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsFrequency = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsFrequency.hertz,                          "hertz",                        "Hz",       mp_one,                             "https://en.wikipedia.org/wiki/Hertz"),
            new LibUCBase(EnumConversionsFrequency.decihertz,                      "decihertz",                    "dHz",      mp_deci,                            "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.centihertz,                     "centihertz",                   "cHz",      mp_centi,                           "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.millihertz,                     "millihertz",                   "mHz",      mp_milli,                           "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.microhertz,                     "microhertz",                   "µHz",      mp_micro,                           "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.nanohertz,                      "nanohertz",                    "nHz",      mp_nano,                            "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.picohertz,                      "picohertz",                    "pHz",      mp_pico,                            "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.femtohertz,                     "femtohertz",                   "fHz",      mp_femto,                           "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.attohertz,                      "attohertz",                    "aHz",      mp_atto,                            "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.zeptohertz,                     "zeptohertz",                   "zHz",      mp_zepto,                           "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.yoctohertz,                     "yoctohertz",                   "yHz",      mp_yocto,                           "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.decahertz,                      "decahertz",                    "daHz",     mp_deca,                            "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.hectohertz,                     "hectohertz",                   "hHz",      mp_hecto,                           "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.kilohertz,                      "kilohertz",                    "kHz",      mp_kilo,                            "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.megahertz,                      "megahertz",                    "MHz",      mp_mega,                            "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.gigahertz,                      "gigahertz",                    "GHz",      mp_giga,                            "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.terahertz,                      "terahertz",                    "THz",      mp_tera,                            "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.petahertz,                      "petahertz",                    "PHz",      mp_peta,                            "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.exahertz,                       "exahertz",                     "EHz",      mp_exa,                             "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.zettahertz,                     "zettahertz",                   "ZHz",      mp_zetta,                           "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.yottahertz,                     "yottahertz",                   "YHz",      mp_yotta,                           "https://en.wikipedia.org/wiki/Hertz#SI_multiples"),
            new LibUCBase(EnumConversionsFrequency.cycle_per_sec,                  "cycle/sec",                    "",         mp_one,                             "https://en.wikipedia.org/wiki/Cycle_per_second"),
            new LibUCBase(EnumConversionsFrequency.cycle_per_min,                  "cycle/min",                    "",         mp_one / timeMin,                   "https://en.wikipedia.org/wiki/Cycle_per_second"),
            new LibUCBase(EnumConversionsFrequency.cycle_per_hour,                 "cycle/hour",                   "",         mp_one / timeHour,                  "https://en.wikipedia.org/wiki/Cycle_per_second"),
            new LibUCBase(EnumConversionsFrequency.cycle_per_day,                  "cycle/day",                    "",         mp_one / timeDay,                   "https://en.wikipedia.org/wiki/Cycle_per_second"),
            new LibUCBase(EnumConversionsFrequency.revolution_per_sec,             "revolution/sec",               "rps",      mp_one,                             "https://en.wikipedia.org/wiki/Revolutions_per_minute"),
            new LibUCBase(EnumConversionsFrequency.revolution_per_min,             "revolution/min",               "rpm",      mp_one / timeMin,                   "https://en.wikipedia.org/wiki/Revolutions_per_minute"),
            new LibUCBase(EnumConversionsFrequency.revolution_per_hour,            "revolution/hour",              "rph",      mp_one / timeHour,                  "https://en.wikipedia.org/wiki/Revolutions_per_minute"),
            new LibUCBase(EnumConversionsFrequency.revolution_per_day,             "revolution/day",               "rpd",      mp_one / timeDay,                   "https://en.wikipedia.org/wiki/Revolutions_per_minute"),
            new LibUCBase(EnumConversionsFrequency.radian_per_sec,                 "radian/sec",                   "rad/s",    mp_one / fpc_pi / 2m,               "https://en.wikipedia.org/wiki/Radian_per_second"),
            new LibUCBase(EnumConversionsFrequency.radian_per_min,                 "radian/min",                   "rad/m",    mp_one / fpc_pi / 2m / timeMin,     "https://en.wikipedia.org/wiki/Radian_per_second"),
            new LibUCBase(EnumConversionsFrequency.radian_per_hour,                "radian/hour",                  "rad/h",    mp_one / fpc_pi / 2m / timeHour,    "https://en.wikipedia.org/wiki/Radian_per_second"),
            new LibUCBase(EnumConversionsFrequency.radian_per_day,                 "radian/day",                   "rad/d",    mp_one / fpc_pi / 2m / timeDay,     "https://en.wikipedia.org/wiki/Radian_per_second"),
            new LibUCBase(EnumConversionsFrequency.degree_per_sec,                 "degree/sec",                   "˚/s",      mp_one / 360m,                      ""),
            new LibUCBase(EnumConversionsFrequency.degree_per_min,                 "degree/min",                   "˚/m",      mp_one / 360m / timeMin,            ""),
            new LibUCBase(EnumConversionsFrequency.degree_per_hour,                "degree/hour",                  "˚/h",      mp_one / 360m / timeHour,           ""),
            new LibUCBase(EnumConversionsFrequency.degree_per_day,                 "degree/day",                   "˚/d",      mp_one / 360m / timeDay,            ""),
            new LibUCBase(EnumConversionsFrequency.wavelengthLight_meter,          "wavelength-light meter",       "",         fpc_c,                              ""),
            new LibUCBase(EnumConversionsFrequency.wavelengthLight_decimeter,      "wavelength-light decimeter",   "",         fpc_c / mp_deci,                    ""),
            new LibUCBase(EnumConversionsFrequency.wavelengthLight_centimeter,     "wavelength-light centimeter",  "",         fpc_c / mp_centi,                   ""),
            new LibUCBase(EnumConversionsFrequency.wavelengthLight_millimeter,     "wavelength-light millimeter",  "",         fpc_c / mp_milli,                   ""),
            new LibUCBase(EnumConversionsFrequency.wavelengthLight_micrometer,     "wavelength-light micrometer",  "",         fpc_c / mp_micro,                   ""),
        };
        #endregion

        /* Conversions Length **************************************************************************************************/

        #region
        public const string StringHyperlinkLength = "https://en.wikipedia.org/wiki/Length";
        // Not used! private const string StringHyperlinkLengthDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Length";

        // one, deci, centi, milli, micro, nano, pico, femto, atto, zepto, yocto, deca, hecto, kilo, mega, giga, tera, peta, exa, zetta, yotta.
        // Set base unit to first item in list.
        /// <summary>
        /// List of available Length conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsLength = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsLength.meter,             "meter",                "m",    mp_one,                     "https://en.wikipedia.org/wiki/Metre"),
            new LibUCBase(EnumConversionsLength.decimeter,         "decimeter",            "dm",   mp_deci,                    "https://en.wikipedia.org/wiki/Decimetre"),
            new LibUCBase(EnumConversionsLength.centimeter,        "centimeter",           "cm",   mp_centi,                   "https://en.wikipedia.org/wiki/Centimetre"),
            new LibUCBase(EnumConversionsLength.millimeter,        "millimeter",           "mm",   mp_milli,                   "https://en.wikipedia.org/wiki/Millimetre"),
            new LibUCBase(EnumConversionsLength.micrometer,        "micrometer",           "µm",   mp_micro,                   "https://en.wikipedia.org/wiki/Micrometre"),
            new LibUCBase(EnumConversionsLength.nanometer,         "nanometer",            "nm",   mp_nano,                    "https://en.wikipedia.org/wiki/Nanometre"),
            new LibUCBase(EnumConversionsLength.picometer,         "picometer",            "pm",   mp_pico,                    "https://en.wikipedia.org/wiki/Picometre"),
            new LibUCBase(EnumConversionsLength.femtometer,        "femtometer",           "fm",   mp_femto,                   "https://en.wikipedia.org/wiki/Femtometre"),
            new LibUCBase(EnumConversionsLength.attometer,         "attometer",            "am",   mp_atto,                    "https://en.wikipedia.org/wiki/Metre#SI_prefixed_forms_of_metre"),
            new LibUCBase(EnumConversionsLength.zeptometer,        "zeptometer",           "zm",   mp_zepto,                   "https://en.wikipedia.org/wiki/Metre#SI_prefixed_forms_of_metre"),
            new LibUCBase(EnumConversionsLength.yoctometer,        "yoctometer",           "ym",   mp_yocto,                   "https://en.wikipedia.org/wiki/Yoctometre"),
            new LibUCBase(EnumConversionsLength.decameter,         "decameter",            "dam",  mp_deca,                    "https://en.wikipedia.org/wiki/Decametre"),
            new LibUCBase(EnumConversionsLength.hectometer,        "hectometer",           "hm",   mp_hecto,                   "https://en.wikipedia.org/wiki/Hectometre"),
            new LibUCBase(EnumConversionsLength.kilometer,         "kilometer",            "km",   mp_kilo,                    "https://en.wikipedia.org/wiki/Kilometre"),
            new LibUCBase(EnumConversionsLength.megameter,         "megameter",            "Mm",   mp_mega,                    "https://en.wikipedia.org/wiki/Megametre"),
            new LibUCBase(EnumConversionsLength.gigameter,         "gigameter",            "Gm",   mp_giga,                    "https://en.wikipedia.org/wiki/Gigametre"),
            new LibUCBase(EnumConversionsLength.terameter,         "terameter",            "Tm",   mp_tera,                    "https://en.wikipedia.org/wiki/Metre#SI_prefixed_forms_of_metre"),
            new LibUCBase(EnumConversionsLength.petameter,         "petameter",            "Pm",   mp_peta,                    "https://en.wikipedia.org/wiki/Metre#SI_prefixed_forms_of_metre"),
            new LibUCBase(EnumConversionsLength.exameter,          "exameter",             "Em",   mp_exa,                     "https://en.wikipedia.org/wiki/Metre#SI_prefixed_forms_of_metre"),
            new LibUCBase(EnumConversionsLength.zettameter,        "zettameter",           "Zm",   mp_zetta,                   "https://en.wikipedia.org/wiki/Metre#SI_prefixed_forms_of_metre"),
            new LibUCBase(EnumConversionsLength.yottameter,        "yottameter",           "Ym",   mp_yotta,                   "https://en.wikipedia.org/wiki/Metre#SI_prefixed_forms_of_metre"),
            new LibUCBase(EnumConversionsLength.angstrom,          "angstrom",             "Å",    0.0000000001m,              "https://en.wikipedia.org/wiki/%C3%85ngstr%C3%B6m"),
            new LibUCBase(EnumConversionsLength.inch,              "inch",                 "in",   lengthInch,                 "https://en.wikipedia.org/wiki/Inch"),
            new LibUCBase(EnumConversionsLength.mil,               "mil",                  "mil",  lengthInch / 1000m,         "https://en.wikipedia.org/wiki/Thousandth_of_an_inch"),
            new LibUCBase(EnumConversionsLength.handBase10,        "hand-base-10",         "h",    lengthInch * 4m,            "https://en.wikipedia.org/wiki/Hand_(unit)"),
            new LibUCBase(EnumConversionsLength.foot,              "foot",                 "ft",   lengthFoot,                 "https://en.wikipedia.org/wiki/Foot_(unit)"),
            new LibUCBase(EnumConversionsLength.rod,               "rod",                  "rd",   lengthFoot * 16.5m,         "https://en.wikipedia.org/wiki/Rod_(unit)"),
            new LibUCBase(EnumConversionsLength.chain,             "chain",                "ch",   lengthFoot * 66m,           "https://en.wikipedia.org/wiki/Chain_(unit)"),
            new LibUCBase(EnumConversionsLength.furlong,           "furlong",              "fur",  lengthFoot * 660m,          "https://en.wikipedia.org/wiki/Furlong"),
            new LibUCBase(EnumConversionsLength.yard,              "yard",                 "yd",   lengthYard,                 "https://en.wikipedia.org/wiki/Yard"),
            new LibUCBase(EnumConversionsLength.fathom,            "fathom",               "ftm",  lengthYard * 2m,            "https://en.wikipedia.org/wiki/Fathom"),
            new LibUCBase(EnumConversionsLength.mile,              "mile",                 "mi",   lengthMile,                 "https://en.wikipedia.org/wiki/Mile"),
            new LibUCBase(EnumConversionsLength.nauticalMile,      "nautical-mile",        "NM",   lengthNauticalMile,         "https://en.wikipedia.org/wiki/Nautical_mile"),
            new LibUCBase(EnumConversionsLength.nauticalMileIMP,   "nautical-mile-IMP",    "NM",   lengthNauticalMileIMP,      "https://en.wikipedia.org/wiki/Nautical_mile"),
            new LibUCBase(EnumConversionsLength.cable,             "cable",                "",     lengthNauticalMile / 10m,   "https://en.wikipedia.org/wiki/Cable_length"),
            new LibUCBase(EnumConversionsLength.astronomicalUnit,  "astronomical-unit",    "AU",   149597870700m,              "https://en.wikipedia.org/wiki/Astronomical_unit"),
            new LibUCBase(EnumConversionsLength.lightSecond,       "light-sec",            "",     fpc_c,                      "https://en.wikipedia.org/wiki/Light-second"),
            new LibUCBase(EnumConversionsLength.lightMinute,       "light-min",            "",     fpc_c * timeMin,            "https://en.wikipedia.org/wiki/Light-second"),
            new LibUCBase(EnumConversionsLength.lightHour,         "light-hour",           "",     fpc_c * timeHour,           "https://en.wikipedia.org/wiki/Light-second"),
            new LibUCBase(EnumConversionsLength.lightDay,          "light-day",            "",     fpc_c * timeDay,            "https://en.wikipedia.org/wiki/Light-second"),
            new LibUCBase(EnumConversionsLength.lightWeek,         "light-week",           "",     fpc_c * timeDay * 7m,       "https://en.wikipedia.org/wiki/Light-second"),
            new LibUCBase(EnumConversionsLength.lightYear,         "light-year",           "ly",   fpc_c * timeDay * 365.25m,  "https://en.wikipedia.org/wiki/Light-year"),
        };
        #endregion

        /* Conversions Mass ****************************************************************************************************/

        #region
        public const string StringHyperlinkMass = "https://en.wikipedia.org/wiki/Mass";
        // Not used! private const string StringHyperlinkMassDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Mass";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Mass conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsMass = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsMass.kilogram,            "kilogram",             "kg",           mp_one,                         "https://en.wikipedia.org/wiki/Kilogram"),
            new LibUCBase(EnumConversionsMass.gram,                "gram",                 "g",            massGram,                       "https://en.wikipedia.org/wiki/Gram"),
            new LibUCBase(EnumConversionsMass.milligram,           "milligram",            "mg",           massGram * mp_milli,            "https://en.wikipedia.org/wiki/Kilogram#SI_multiples"),
            new LibUCBase(EnumConversionsMass.microgram,           "microgram",            "µg",           massGram * mp_micro,            "https://en.wikipedia.org/wiki/Kilogram#SI_multiples"),
            new LibUCBase(EnumConversionsMass.megagram,            "megagram",             "Mg",           massGram * mp_mega,             "https://en.wikipedia.org/wiki/Kilogram#SI_multiples"),
            new LibUCBase(EnumConversionsMass.carat,               "carat",                "ct",           massGram * mp_milli * 200m,     "https://en.wikipedia.org/wiki/Carat_(mass)"),
            new LibUCBase(EnumConversionsMass.poundMetric,         "pound-metric",         "",             massGram * 500m,                "https://en.wikipedia.org/wiki/Pound_(mass)#Metric_pounds"),
            new LibUCBase(EnumConversionsMass.ounce,               "ounce",                "oz",           massOunce,                      "https://en.wikipedia.org/wiki/Ounce"),
            new LibUCBase(EnumConversionsMass.dram,                "dram",                 "dr",           massOunce / 16m,                "https://en.wikipedia.org/wiki/Avoirdupois#American_customary_system"),
            new LibUCBase(EnumConversionsMass.pound,               "pound",                "lb",           massPound,                      "https://en.wikipedia.org/wiki/Pound_(mass)"),
            new LibUCBase(EnumConversionsMass.grain,               "grain",                "gr",           massPound / 7000m,              "https://en.wikipedia.org/wiki/Grain_(unit)"),
            new LibUCBase(EnumConversionsMass.stone,               "stone",                "st",           massPound * 14m,                "https://en.wikipedia.org/wiki/Stone_(unit)"),
            new LibUCBase(EnumConversionsMass.quaterUS,            "quater-US",            "qr US",        massPound * 25m,                "https://en.wikipedia.org/wiki/Avoirdupois#American_customary_system"),
            new LibUCBase(EnumConversionsMass.quaterIMP,           "quater-IMP",           "qr IMP",       massPound * 28m,                "https://en.wikipedia.org/wiki/Avoirdupois#Post-Elizabethan"),
            new LibUCBase(EnumConversionsMass.cental,              "cental",               "",             massPound * 100m,               "https://en.wikipedia.org/wiki/Imperial_and_US_customary_measurement_systems#Avoirdupois_system"),
            new LibUCBase(EnumConversionsMass.hundredweightUS,     "hundredweight-US",     "cwt US",       massPound * 100m,               "https://en.wikipedia.org/wiki/Avoirdupois#American_customary_system"),
            new LibUCBase(EnumConversionsMass.hundredweightIMP,    "hundredweight-IMP",    "cwt IMP",      massPound * 112m,               "https://en.wikipedia.org/wiki/Avoirdupois#Post-Elizabethan"),
            new LibUCBase(EnumConversionsMass.kip,                 "kip",                  "kip",          massPound * 1000m,              "https://en.wikipedia.org/wiki/Kip_(unit)"),
            new LibUCBase(EnumConversionsMass.tonUS,               "ton-US",               "ton short",    massPound * 2000m,              "https://en.wikipedia.org/wiki/Short_ton"),
            new LibUCBase(EnumConversionsMass.tonIMP,              "ton-IMP",              "ton long",     massPound * 2240m,              "https://en.wikipedia.org/wiki/Long_ton"),
            new LibUCBase(EnumConversionsMass.poundTroy,           "pound-troy",           "lb t",         massPoundTroy,                  "https://en.wikipedia.org/wiki/Pound_(mass)#Troy_pound"),
            new LibUCBase(EnumConversionsMass.ounceTroy,           "ounce-troy",           "oz t",         massPoundTroy / 12m,            "https://en.wikipedia.org/wiki/Troy_ounce"),
            new LibUCBase(EnumConversionsMass.pennyweight,         "pennyweight",          "pwt",          massPound / 240m,               "https://en.wikipedia.org/wiki/Pennyweight"),
            new LibUCBase(EnumConversionsMass.slug,                "slug",                 "slug",         massSlug,                       "https://en.wikipedia.org/wiki/Slug_(mass)"),
        };
        #endregion

        /* Conversions Metric **************************************************************************************************/

        #region
        public const string StringHyperlinkMetric = "https://en.wikipedia.org/wiki/Metric_prefix";
        private const string StringHyperlinkMetricDefault = "https://en.wikipedia.org/wiki/Metric_prefix";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Metric conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsMetric = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsMetric.one,       "one",      "",     mp_one,     StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.deci,      "deci",     "d",    mp_deci,    StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.centi,     "centi",    "c",    mp_centi,   StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.milli,     "milli",    "m",    mp_milli,   StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.micro,     "micro",    "µ",    mp_micro,   StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.nano,      "nano",     "n",    mp_nano,    StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.pico,      "pico",     "p",    mp_pico,    StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.femto,     "femto",    "f",    mp_femto,   StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.atto,      "atto",     "a",    mp_atto,    StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.zepto,     "zepto",    "z",    mp_zepto,   StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.yocto,     "yocto",    "y",    mp_yocto,   StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.deca,      "deca",     "da",   mp_deca,    StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.hecto,     "hecto",    "h",    mp_hecto,   StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.kilo,      "kilo",     "k",    mp_kilo,    StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.mega,      "mega",     "M",    mp_mega,    StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.giga,      "giga",     "G",    mp_giga,    StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.tera,      "tera",     "T",    mp_tera,    StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.peta,      "peta",     "P",    mp_peta,    StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.exa,       "exa",      "E",    mp_exa,     StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.zetta,     "zetta",    "Z",    mp_zetta,   StringHyperlinkMetricDefault),
            new LibUCBase(EnumConversionsMetric.yotta,     "yotta",    "Y",    mp_yotta,   StringHyperlinkMetricDefault),
        };
        #endregion

        /* Conversions Power ***************************************************************************************************/

        #region
        public const string StringHyperlinkPower = "https://en.wikipedia.org/wiki/Power_(physics)";
        private const string StringHyperlinkPowerDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Power_or_heat_flow_rate";

        // one, deci, centi, milli, micro, nano, pico, femto, atto, zepto, yocto, deca, hecto, kilo, mega, giga, tera, peta, exa, zetta, yotta.
        // Set base unit to first item in list.
        /// <summary>
        /// List of available Power conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsPower = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsPower.watt,                                   "watt",                             "W",            mp_one,                                                 "https://en.wikipedia.org/wiki/Watt"),
            new LibUCBase(EnumConversionsPower.joule_per_sec,                          "joule/sec",                        "J/s",          mp_one,                                                 "https://en.wikipedia.org/wiki/Watt"),
            new LibUCBase(EnumConversionsPower.newton_meter_per_sec,                   "newton·meter/sec",                 "N·m/s",        mp_one,                                                 "https://en.wikipedia.org/wiki/Watt"),
            new LibUCBase(EnumConversionsPower.kilogram_meter_squared_per_sec_cubed,   "kilogram·meter²/sec³",             "kg·m²/s³",     mp_one,                                                 "https://en.wikipedia.org/wiki/Watt"),
            new LibUCBase(EnumConversionsPower.milliwatt,                              "milliwatt",                        "mW",           mp_milli,                                               "https://en.wikipedia.org/wiki/Watt"),
            new LibUCBase(EnumConversionsPower.microwatt,                              "microwatt",                        "µW",           mp_micro,                                               "https://en.wikipedia.org/wiki/Watt"),
            new LibUCBase(EnumConversionsPower.nanowatt,                               "nanowatt",                         "nW",           mp_nano,                                                "https://en.wikipedia.org/wiki/Watt"),
            new LibUCBase(EnumConversionsPower.picowatt,                               "picowatt",                         "pW",           mp_pico,                                                "https://en.wikipedia.org/wiki/Watt"),
            new LibUCBase(EnumConversionsPower.kilowatt,                               "kilowatt",                         "kW",           mp_kilo,                                                "https://en.wikipedia.org/wiki/Watt"),
            new LibUCBase(EnumConversionsPower.megawatt,                               "megawatt",                         "MW",           mp_mega,                                                "https://en.wikipedia.org/wiki/Watt"),
            new LibUCBase(EnumConversionsPower.gigawatt,                               "gigawatt",                         "GW",           mp_giga,                                                "https://en.wikipedia.org/wiki/Watt"),
            new LibUCBase(EnumConversionsPower.terawatt,                               "terawatt",                         "TW",           mp_tera,                                                "https://en.wikipedia.org/wiki/Watt"),
            new LibUCBase(EnumConversionsPower.atmosphere_liter_per_sec,               "atmosphere·liter/sec",             "atm·L/s",      pressureAtmosphere * volumeLiter,                       StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.atmosphere_liter_per_min,               "atmosphere·liter/min",             "atm·L/m",      pressureAtmosphere * volumeLiter / timeMin,             StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.atmosphere_liter_per_hour,              "atmosphere·liter/hour",            "atm·L/h",      pressureAtmosphere * volumeLiter / timeHour,            ""),
            new LibUCBase(EnumConversionsPower.atmosphere_meter_cubed_per_sec,         "atmosphere·meter³/sec",            "atm·m³/s",     pressureAtmosphere,                                     ""),
            new LibUCBase(EnumConversionsPower.atmosphere_meter_cubed_per_min,         "atmosphere·meter³/min",            "atm·m³/m",     pressureAtmosphere / timeMin,                           ""),
            new LibUCBase(EnumConversionsPower.atmosphere_meter_cubed_per_hour,        "atmosphere·meter³/hour",           "atm·m³/h",     pressureAtmosphere / timeHour,                          ""),
            new LibUCBase(EnumConversionsPower.atmosphere_centimeter_cubed_per_sec,    "atmosphere·cm³/sec",               "atm·cm³/s",    pressureAtmosphere * volumeCentimeterCubed,             StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.atmosphere_centimeter_cubed_per_min,    "atmosphere·cm³/min",               "atm·cm³/m",    pressureAtmosphere * volumeCentimeterCubed / timeMin,   StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.atmosphere_centimeter_cubed_per_hour,   "atmosphere·cm³/hour",              "atm·cm³/h",    pressureAtmosphere * volumeCentimeterCubed / timeHour,  ""),
            new LibUCBase(EnumConversionsPower.atmosphere_inch_cubed_per_sec,          "atmosphere·inch³/sec",             "atm·in³/s",    pressureAtmosphere * volumeInchCubed,                   ""),
            new LibUCBase(EnumConversionsPower.atmosphere_inch_cubed_per_min,          "atmosphere·inch³/min",             "atm·in³/m",    pressureAtmosphere * volumeInchCubed / timeMin,         ""),
            new LibUCBase(EnumConversionsPower.atmosphere_inch_cubed_per_hour,         "atmosphere·inch³/hour",            "atm·in³/h",    pressureAtmosphere * volumeInchCubed / timeHour,        ""),
            new LibUCBase(EnumConversionsPower.atmosphere_foot_cubed_per_sec,          "atmosphere·foot³/sec",             "atm·cfs",      pressureAtmosphere * volumeFootCubed,                   StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.atmosphere_foot_cubed_per_min,          "atmosphere·foot³/min",             "atm·cfm",      pressureAtmosphere * volumeFootCubed / timeMin,         StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.atmosphere_foot_cubed_per_hour,         "atmosphere·foot³/hour",            "atm·cfh",      pressureAtmosphere * volumeFootCubed / timeHour,        StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.lusec,                                  "lusec",                            "lusec",        pressureAtmosphere / 760m * mp_milli * volumeLiter,     StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.newton_meter_per_min,                   "newton·meter/min",                 "N·m/m",        mp_one / timeMin,                                       ""),
            new LibUCBase(EnumConversionsPower.newton_meter_per_hour,                  "newton·meter/hour",                "N·m/h",        mp_one / timeHour,                                      ""),
            new LibUCBase(EnumConversionsPower.gramForce_centimeter_per_sec,           "gram-force·cm/sec",                "gf·cm/s",      forceKilogram * massGram * mp_centi,                    ""),
            new LibUCBase(EnumConversionsPower.gramForce_centimeter_per_min,           "gram-force·cm/min",                "gf·cm/m",      forceKilogram * massGram * mp_centi / timeMin,          ""),
            new LibUCBase(EnumConversionsPower.gramForce_centimeter_per_hour,          "gram-force·cm/hour",               "gf·cm/h",      forceKilogram * massGram * mp_centi / timeHour,         ""),
            new LibUCBase(EnumConversionsPower.kilogramForce_meter_per_sec,            "kilogram-force·meter/sec",         "kgf·m/s",      forceKilogram,                                          ""),
            new LibUCBase(EnumConversionsPower.kilogramForce_meter_per_min,            "kilogram-force·meter/min",         "kgf·m/m",      forceKilogram / timeMin,                                ""),
            new LibUCBase(EnumConversionsPower.kilogramForce_meter_per_hour,           "kilogram-force·meter/hour",        "kgf·m/h",      forceKilogram / timeHour,                               ""),
            new LibUCBase(EnumConversionsPower.poncelet,                               "poncelet",                         "p",            forceKilogram * 100m,                                   "https://en.wikipedia.org/wiki/Poncelet"),
            new LibUCBase(EnumConversionsPower.erg_per_sec,                            "erg/sec",                          "erg/sec",      0.0000001m,                                             StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.calorieIT_per_sec,                      "calorie-IT/sec",                   "cal-IT/s",     energyCalorieIT,                                        StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.calorieIT_per_min,                      "calorie-IT/min",                   "cal-IT/m",     energyCalorieIT / timeMin,                              ""),
            new LibUCBase(EnumConversionsPower.calorieIT_per_hour,                     "calorie-IT/hour",                  "cal-IT/h",     energyCalorieIT / timeHour,                             ""),
            new LibUCBase(EnumConversionsPower.calorieTH_per_sec,                      "calorie-TH/sec",                   "cal-TH/s",     energyCalorieTH,                                        ""),
            new LibUCBase(EnumConversionsPower.calorieTH_per_min,                      "calorie-TH/min",                   "cal-TH/m",     energyCalorieTH / timeMin,                              ""),
            new LibUCBase(EnumConversionsPower.calorieTH_per_hour,                     "calorie-TH/hour",                  "cal-TH/h",     energyCalorieTH / timeHour,                             ""),
            new LibUCBase(EnumConversionsPower.btuIT_per_sec,                          "btu-IT/sec",                       "BTU-IT/s",     energyBtuIT,                                            StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.btuIT_per_min,                          "btu-IT/min",                       "BTU-IT/m",     energyBtuIT / timeMin,                                  StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.btuIT_per_hour,                         "btu-IT/hour",                      "BTU-IT/h",     energyBtuIT / timeHour,                                 StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.foot_squared_equiv_direct_radiation,    "foot² equiv direct radiation",     "ft² EDR",      energyBtuIT * 240m / timeHour,                          "https://en.wikipedia.org/wiki/Equivalence_of_direct_radiation"),
            new LibUCBase(EnumConversionsPower.btuTH_per_sec,                          "btu-TH/sec",                       "BTU-TH/s",     energyBtuTH,                                            ""),
            new LibUCBase(EnumConversionsPower.btuTH_per_min,                          "btu-TH/min",                       "BTU-Th/m",     energyBtuTH / timeMin,                                  ""),
            new LibUCBase(EnumConversionsPower.btuTH_per_hour,                         "btu-TH/hour",                      "BTU-TH/h",     energyBtuTH / timeHour,                                 ""),
            new LibUCBase(EnumConversionsPower.ounceForce_foot_per_sec,                "ounce-force·foot/sec",             "ozf·ft/s",     forceOunce * lengthFoot,                                ""),
            new LibUCBase(EnumConversionsPower.ounceForce_foot_per_min,                "ounce-force·foot/min",             "ozf·ft/m",     forceOunce * lengthFoot / timeMin,                      ""),
            new LibUCBase(EnumConversionsPower.ounceForce_foot_per_hour,               "ounce-force·foot/hour",            "ozf·ft/h",     forceOunce * lengthFoot / timeHour,                     ""),
            new LibUCBase(EnumConversionsPower.poundForce_foot_per_sec,                "pound-force·foot/sec",             "lbf·ft/s",     forcePound * lengthFoot,                                StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.poundForce_foot_per_min,                "pound-force·foot/min",             "lbf·ft/m",     forcePound * lengthFoot / timeMin,                      StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.poundForce_foot_per_hour,               "pound-force·foot/hour",            "lbf·ft/h",     forcePound * lengthFoot / timeHour,                     StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.poundal_foot_per_sec,                   "poundal·foot/sec",                 "pdl·ft/s",     forcePoundal * lengthFoot,                              ""),
            new LibUCBase(EnumConversionsPower.poundal_foot_per_min,                   "poundal·foot/min",                 "pdl·ft/m",     forcePoundal * lengthFoot / timeMin,                    ""),
            new LibUCBase(EnumConversionsPower.poundal_foot_per_hour,                  "poundal·foot/hour",                "pdl·ft/h",     forcePoundal * lengthFoot / timeHour,                   ""),
            new LibUCBase(EnumConversionsPower.horsepowerBoiler,                       "horsepower-boiler",                "BHP",          powerHorsepowerBoiler,                                  StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.horsepowerElectric,                     "horsepower-electric",              "hp",           powerHorsepowerElectric,                                StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.horsepowerMechanical,                   "horsepower-mechanical",            "hp",           powerHorsepowerMechanical,                              StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.horsepowerMetric,                       "horsepower-metric",                "hp",           powerHorsepowerMetric,                                  StringHyperlinkPowerDefault),
            new LibUCBase(EnumConversionsPower.refrigeration_pound,                    "refrigeration pound",              "",             powerRefrigerationPound,                                ""),
            new LibUCBase(EnumConversionsPower.refrigeration_tonUS,                    "refrigeration ton-US",             "RT ton short", powerRefrigerationPound * 2000m,                        "https://en.wikipedia.org/wiki/Ton_of_refrigeration"),
            new LibUCBase(EnumConversionsPower.refrigeration_tonIMP,                   "refrigeration ton-IMP",            "RT ton long",  powerRefrigerationPound * 2240m,                        ""),
        };
        #endregion

        /* Conversions Pressure ************************************************************************************************/

        #region
        public const string StringHyperlinkPressure = "https://en.wikipedia.org/wiki/Pressure";
        private const string StringHyperlinkPressureDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Pressure_or_mechanical_stress";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Density conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsPressure = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsPressure.pascal,                                  "pascal",                       "Pa",               mp_one,                                 "https://en.wikipedia.org/wiki/Pascal_(unit)"),
            new LibUCBase(EnumConversionsPressure.newton_per_meter_squared,                "newton/meter²",                "N/m²",             mp_one,                                 "https://en.wikipedia.org/wiki/Pascal_(unit)"),
            new LibUCBase(EnumConversionsPressure.kilogram_per_meter_per_sec_squared,      "kilogram/meter/sec²",          "kg/m/s²",          mp_one,                                 "https://en.wikipedia.org/wiki/Pascal_(unit)"),
            new LibUCBase(EnumConversionsPressure.decipascal,                              "decipascal",                   "dPa",              mp_deci,                                "https://en.wikipedia.org/wiki/Pascal_(unit)"),
            new LibUCBase(EnumConversionsPressure.centipascal,                             "centipascal",                  "cPa",              mp_centi,                               "https://en.wikipedia.org/wiki/Pascal_(unit)"),
            new LibUCBase(EnumConversionsPressure.millipascal,                             "millipascal",                  "mPa",              mp_milli,                               "https://en.wikipedia.org/wiki/Pascal_(unit)"),
            new LibUCBase(EnumConversionsPressure.micropascal,                             "micropascal",                  "µPa",              mp_micro,                               "https://en.wikipedia.org/wiki/Pascal_(unit)"),
            new LibUCBase(EnumConversionsPressure.hectopascal,                             "hectopascal",                  "hPa",              mp_hecto,                               "https://en.wikipedia.org/wiki/Pascal_(unit)"),
            new LibUCBase(EnumConversionsPressure.kilopascal,                              "kilopascal",                   "kPa",              mp_kilo,                                "https://en.wikipedia.org/wiki/Pascal_(unit)"),
            new LibUCBase(EnumConversionsPressure.megapascal,                              "megapascal",                   "MPa",              mp_mega,                                "https://en.wikipedia.org/wiki/Pascal_(unit)"),
            new LibUCBase(EnumConversionsPressure.gigapascal,                              "gigapascal",                   "GPa",              mp_giga,                                "https://en.wikipedia.org/wiki/Pascal_(unit)"),
            new LibUCBase(EnumConversionsPressure.bar,                                     "bar",                          "bar",              pressureBar,                            "https://en.wikipedia.org/wiki/Bar_(unit)#Usage"),
            new LibUCBase(EnumConversionsPressure.millibar,                                "millibar",                     "mbar",             pressureBar * mp_milli,                 "https://en.wikipedia.org/wiki/Bar_(unit)#Usage"),
            new LibUCBase(EnumConversionsPressure.microbar,                                "microbar",                     "µbar",             pressureBar * mp_micro,                 "https://en.wikipedia.org/wiki/Bar_(unit)#Usage"),
            new LibUCBase(EnumConversionsPressure.barye,                                   "barye",                        "Ba",               forceDyne / (mp_centi * mp_centi),      "https://en.wikipedia.org/wiki/Barye"),
            new LibUCBase(EnumConversionsPressure.dyne_per_centimeter_squared,             "dyne/centimeter²",             "dyn/cm²",          forceDyne / (mp_centi * mp_centi),      ""),
            new LibUCBase(EnumConversionsPressure.atmosphereStandard,                      "atmosphere-standard",          "atm",              pressureAtmosphere,                     "https://en.wikipedia.org/wiki/Atmosphere_(unit)"),
            new LibUCBase(EnumConversionsPressure.atmosphereTechnical,                     "atmosphere-technical",         "at",               forceKilogram / (mp_centi * mp_centi),  "https://en.wikipedia.org/wiki/Technical_atmosphere"),
            new LibUCBase(EnumConversionsPressure.torr,                                    "torr",                         "torr ",            pressureAtmosphere / 760m,              "https://en.wikipedia.org/wiki/Torr"),
            new LibUCBase(EnumConversionsPressure.poundForce_per_inch_squared,             "pound-force/inch²",            "psi",              forcePound / areaInchSquared,           StringHyperlinkPressureDefault),
            new LibUCBase(EnumConversionsPressure.poundForce_per_foot_squared,             "pound-force/foot²",            "psf",              forcePound / areaFootSquared,           StringHyperlinkPressureDefault),
            new LibUCBase(EnumConversionsPressure.kipForce_per_inch_squared,               "kip-force/inch²",              "ksi",              forcePound * 1000m / areaInchSquared,   ""),
            new LibUCBase(EnumConversionsPressure.kipForce_per_foot_squared,               "kip-force/foot²",              "ksf",              forcePound * 1000m / areaFootSquared,   ""),
            new LibUCBase(EnumConversionsPressure.tonForceUS_per_foot_squared,             "ton-force-US/foot²",           "tnf short/ft²",    forcePound * 2000m / areaFootSquared,   StringHyperlinkPressureDefault),
            new LibUCBase(EnumConversionsPressure.tonForceIMP_per_foot_squared,            "ton-force-IMP/foot²",          "tnf long/ft²",     forcePound * 2240m / areaFootSquared,   ""),
            new LibUCBase(EnumConversionsPressure.poundal_per_inch_squared,                "poundal/inch²",                "pdl/in²",          forcePoundal / areaInchSquared,         ""),
            new LibUCBase(EnumConversionsPressure.poundal_per_foot_squared,                "poundal/foot²",                "pdl/ft²",          forcePoundal / areaFootSquared,         StringHyperlinkPressureDefault),
            new LibUCBase(EnumConversionsPressure.kilogramForce_per_meter_squared,         "kilogram-force/meter²",        "kgf/m²",           forceKilogram,                          ""),
            new LibUCBase(EnumConversionsPressure.kilogramForce_per_centimeter_squared,    "kilogram-force/centimeter²",   "kgf/cm²",          forceKilogram / (mp_centi * mp_centi),  ""),
            new LibUCBase(EnumConversionsPressure.kilogramForce_per_millimeter_squared,    "kilogram-force/millimeter²",   "kgf/mm²",          forceKilogram / (mp_milli * mp_milli),  StringHyperlinkPressureDefault),
            new LibUCBase(EnumConversionsPressure.mercury_meter,                           "mercury·meter",                "Hg·m",             pressureMercury,                        "https://en.wikipedia.org/wiki/Millimeter_of_mercury"),
            new LibUCBase(EnumConversionsPressure.mercury_centimeter,                      "mercury·centimeter",           "Hg·cm",            pressureMercury * mp_centi,             "https://en.wikipedia.org/wiki/Millimeter_of_mercury"),
            new LibUCBase(EnumConversionsPressure.mercury_millimeter,                      "mercury·millimeter",           "Hg·mm",            pressureMercury * mp_milli,             "https://en.wikipedia.org/wiki/Millimeter_of_mercury"),
            new LibUCBase(EnumConversionsPressure.mercury_inch,                            "mercury·inch",                 "Hg·in",            pressureMercury * lengthInch,           "https://en.wikipedia.org/wiki/Millimeter_of_mercury"),
            new LibUCBase(EnumConversionsPressure.mercury_foot,                            "mercury·foot",                 "Hg.ft",            pressureMercury * lengthFoot,           "https://en.wikipedia.org/wiki/Millimeter_of_mercury"),
            new LibUCBase(EnumConversionsPressure.waterC4_meter,                           "water-4°C·meter",              "H₂O-4C·m",         pressureWaterC4,                        "https://en.wikipedia.org/wiki/Centimetre_of_water"),
            new LibUCBase(EnumConversionsPressure.waterC4_centimeter,                      "water-4°C·centimeter",         "H₂O-4C·cm",        pressureWaterC4 * mp_centi,             "https://en.wikipedia.org/wiki/Centimetre_of_water"),
            new LibUCBase(EnumConversionsPressure.waterC4_millimeter,                      "water-4°C·millimeter",         "H₂O-4C·mm",        pressureWaterC4 * mp_milli,             "https://en.wikipedia.org/wiki/Centimetre_of_water"),
            new LibUCBase(EnumConversionsPressure.waterC4_inch,                            "water-4°C·inch",               "H₂O-4C·in",        pressureWaterC4 * lengthInch,           "https://en.wikipedia.org/wiki/Inch_of_water"),
            new LibUCBase(EnumConversionsPressure.waterC4_foot,                            "water-4°C·foot",               "H₂O-4C·ft",        pressureWaterC4 * lengthFoot,           "https://en.wikipedia.org/wiki/Centimetre_of_water"),
            new LibUCBase(EnumConversionsPressure.waterC15_meter,                          "water-15°C·meter",             "H₂O-15C·m",        pressureWaterC15,                       "https://en.wikipedia.org/wiki/Centimetre_of_water"),
            new LibUCBase(EnumConversionsPressure.waterC15_centimeter,                     "water-15°C·centimeter",        "H₂O-15C·cm",       pressureWaterC15 * mp_centi,            "https://en.wikipedia.org/wiki/Centimetre_of_water"),
            new LibUCBase(EnumConversionsPressure.waterC15_millimeter,                     "water-15°C·millimeter",        "H₂O-15C·mm",       pressureWaterC15 * mp_milli,            "https://en.wikipedia.org/wiki/Centimetre_of_water"),
            new LibUCBase(EnumConversionsPressure.waterC15_inch,                           "water-15°C·inch",              "H₂O-15C·in",       pressureWaterC15 * lengthInch,          "https://en.wikipedia.org/wiki/Inch_of_water"),
            new LibUCBase(EnumConversionsPressure.waterC15_foot,                           "water-15°C·foot",              "H₂O-15C·ft",       pressureWaterC15 * lengthFoot,          "https://en.wikipedia.org/wiki/Centimetre_of_water"),
        };
        #endregion

        /* Conversions Solid Angle *********************************************************************************************/

        #region
        public const string StringHyperlinkSolidAngle = "https://en.wikipedia.org/wiki/Solid_angle";
        // private const string StringHyperlinkSolidAngleDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Solid_angle";    // Not used so comment out to prevent compiler warning.

        // More about Solid Angles at: http://www.numericana.com/answer/angles.htm#solid 
        // Set base unit to first item in list.
        /// <summary>
        /// List of available Solid Angle conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsSolidAngle = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsSolidAngle.steradian,         "steradian",    "sr",       mp_one,                                                 "https://en.wikipedia.org/wiki/Steradian"),
            new LibUCBase(EnumConversionsSolidAngle.degree_squared,    "degree²",      "deg²",     fpc_pi * fpc_pi / (180m * 180m),                        "https://en.wikipedia.org/wiki/Square_degree"),
            new LibUCBase(EnumConversionsSolidAngle.minute_squared,    "minute²",      "min²",     fpc_pi * fpc_pi / (180m * 180m * timeMin * timeMin),    ""),
            new LibUCBase(EnumConversionsSolidAngle.second_squared,    "second²",      "sec²",     fpc_pi * fpc_pi / (180m * 180m * timeHour * timeHour),  ""),
            new LibUCBase(EnumConversionsSolidAngle.hemisphere,        "hemisphere",   "",         fpc_pi * 2m,                                            ""),
            new LibUCBase(EnumConversionsSolidAngle.sphere,            "sphere",       "",         fpc_pi * 4m,                                            ""),
            new LibUCBase(EnumConversionsSolidAngle.spat,              "spat",         "sp",       fpc_pi * 4m,                                            "https://en.wikipedia.org/wiki/Spat_(unit)"),
        };
        #endregion

        /* Conversions Speed ***************************************************************************************************/

        #region
        public const string StringHyperlinkSpeed = "https://en.wikipedia.org/wiki/Speed";
        private const string StringHyperlinkSpeedDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Speed_or_velocity";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Speed conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsSpeed = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsSpeed.meter_per_sec,          "meter/sec",            "m/s",      mp_one,                 "https://en.wikipedia.org/wiki/Metre_per_second"),
            new LibUCBase(EnumConversionsSpeed.meter_per_min,          "meter/min",            "m/m",      mp_one / timeMin,       "https://en.wikipedia.org/wiki/Metre_per_second"),
            new LibUCBase(EnumConversionsSpeed.meter_per_hour,         "meter/hour",           "m/h",      mp_one / timeHour,      "https://en.wikipedia.org/wiki/Metre_per_hour"),
            new LibUCBase(EnumConversionsSpeed.meter_per_day,          "meter/day",            "m/d",      mp_one / timeDay,       "https://en.wikipedia.org/wiki/Metre_per_hour"),
            new LibUCBase(EnumConversionsSpeed.centimeter_per_sec,     "centimeter/sec",       "cm/s",     mp_centi,               "https://en.wikipedia.org/wiki/Metre_per_second"),
            new LibUCBase(EnumConversionsSpeed.centimeter_per_min,     "centimeter/min",       "cm/m",     mp_centi / timeMin,     "https://en.wikipedia.org/wiki/Metre_per_second"),
            new LibUCBase(EnumConversionsSpeed.centimeter_per_hour,    "centimeter/hour",      "cm/h",     mp_centi / timeHour,    "https://en.wikipedia.org/wiki/Metre_per_hour"),
            new LibUCBase(EnumConversionsSpeed.centimeter_per_day,     "centimeter/day",       "cm/d",     mp_centi / timeDay,     "https://en.wikipedia.org/wiki/Metre_per_hour"),
            new LibUCBase(EnumConversionsSpeed.kilometer_per_sec,      "kilometer/sec",        "km/s",     mp_kilo,                "https://en.wikipedia.org/wiki/Kilometres_per_hour"),
            new LibUCBase(EnumConversionsSpeed.kilometer_per_min,      "kilometer/min",        "km/m",     mp_kilo / timeMin,      "https://en.wikipedia.org/wiki/Kilometres_per_hour"),
            new LibUCBase(EnumConversionsSpeed.kilometer_per_hour,     "kilometer/hour",       "km/h",     mp_kilo / timeHour,     "https://en.wikipedia.org/wiki/Kilometres_per_hour"),
            new LibUCBase(EnumConversionsSpeed.kilometer_per_day,      "kilometer/day",        "km/d",     mp_kilo / timeDay,      "https://en.wikipedia.org/wiki/Kilometres_per_hour"),  
            new LibUCBase(EnumConversionsSpeed.inch_per_sec,           "inch/sec",             "ips",      lengthInch,             StringHyperlinkSpeedDefault),
            new LibUCBase(EnumConversionsSpeed.inch_per_min,           "inch/min",             "ipm",      lengthInch / timeMin,   StringHyperlinkSpeedDefault),
            new LibUCBase(EnumConversionsSpeed.inch_per_hour,          "inch/hour",            "iph",      lengthInch / timeHour,  StringHyperlinkSpeedDefault),
            new LibUCBase(EnumConversionsSpeed.inch_per_day,           "inch/day",             "ipd",      lengthInch / timeDay,   StringHyperlinkSpeedDefault),
            new LibUCBase(EnumConversionsSpeed.foot_per_sec,           "foot/sec",             "fps",      lengthFoot,             StringHyperlinkSpeedDefault),
            new LibUCBase(EnumConversionsSpeed.foot_per_min,           "foot/min",             "fpm",      lengthFoot/ timeMin,    StringHyperlinkSpeedDefault),
            new LibUCBase(EnumConversionsSpeed.foot_per_hour,          "foot/hour",            "fph",      lengthFoot / timeHour,  StringHyperlinkSpeedDefault),
            new LibUCBase(EnumConversionsSpeed.foot_per_day,           "foot/day",             "fpd",      lengthFoot / timeDay,   StringHyperlinkSpeedDefault),
            new LibUCBase(EnumConversionsSpeed.mile_per_sec,           "mile/sec",             "mps",      lengthMile,             "https://en.wikipedia.org/wiki/Miles_per_hour"),
            new LibUCBase(EnumConversionsSpeed.mile_per_min,           "mile/min",             "mpm",      lengthMile / timeMin,   "https://en.wikipedia.org/wiki/Miles_per_hour"),
            new LibUCBase(EnumConversionsSpeed.mile_per_hour,          "mile/hour",            "mph",      lengthMile / timeHour,  "https://en.wikipedia.org/wiki/Miles_per_hour"),
            new LibUCBase(EnumConversionsSpeed.mile_per_day,           "mile/day",             "mpd",      lengthMile / timeDay,   "https://en.wikipedia.org/wiki/Miles_per_hour"),
            new LibUCBase(EnumConversionsSpeed.knot,                   "knot",                 "kn",       speedKnot,              "https://en.wikipedia.org/wiki/Knot_(unit)"),
            new LibUCBase(EnumConversionsSpeed.knotIMP,                "knot-IMP",             "kn-IMP",   speedKnotIMP,           "https://en.wikipedia.org/wiki/Knot_(unit)"),
            new LibUCBase(EnumConversionsSpeed.speedMach,              "speed-mach-number",    "M",        340.3m,                 "https://en.wikipedia.org/wiki/Mach_number"),
            new LibUCBase(EnumConversionsSpeed.speedSound,             "speed-sound",          "s",        343.2m,                 "https://en.wikipedia.org/wiki/Speed_of_sound"),
            new LibUCBase(EnumConversionsSpeed.speedLight,             "speed-light",          "c",        fpc_c,                  "https://en.wikipedia.org/wiki/Speed_of_light"),
        };
        #endregion

        /* Conversions Temperature *********************************************************************************************/

        #region
        public const string StringHyperlinkTemperature = "https://en.wikipedia.org/wiki/Temperature";
        // Not used! private const string StringHyperlinkTemperatureDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Temperature";

        // Set base unit to first item in list.
        // Temperature conversions are special case since cannot be completed with simple mutiplication factor.
        // They also require subtraction and addition.
        // Use conversion methods ConvertTemperatureToKelvin() and ConvertTemperatureFromKelvin() to convert values.
        // All DecimalBase values set to 1m and are not used.
        /// <summary>
        /// List of available Temperature conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsTemperature = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsTemperature.kelvin,       "kelvin",       "K",    mp_one,     "https://en.wikipedia.org/wiki/Kelvin"),
            new LibUCBase(EnumConversionsTemperature.celsius,      "celsius",      "°C",   mp_one,     "https://en.wikipedia.org/wiki/Celsius"),
            new LibUCBase(EnumConversionsTemperature.fahrenheit,   "fahrenheit",   "°F",   mp_one,     "https://en.wikipedia.org/wiki/Fahrenheit"),
            new LibUCBase(EnumConversionsTemperature.rankine,      "rankine",      "°R",   mp_one,     "https://en.wikipedia.org/wiki/Rankine_scale"),
        };
        #endregion

        /* Conversions Time ****************************************************************************************************/

        #region
        public const string StringHyperlinkTime = "https://en.wikipedia.org/wiki/Time";
        // Not used! private static string StringHyperlinkTimeDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Time";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Time conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsTime = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsTime.second,              "second",               "s",        mp_one,                     "https://en.wikipedia.org/wiki/Second"),
            new LibUCBase(EnumConversionsTime.decisecond,          "decisecond",           "ds",       mp_deci,                    "https://en.wikipedia.org/wiki/Second"),
            new LibUCBase(EnumConversionsTime.centisecond,         "centisecond",          "cs",       mp_centi,                   "https://en.wikipedia.org/wiki/Second"),
            new LibUCBase(EnumConversionsTime.millisecond,         "millisecond",          "ms",       mp_milli,                   "https://en.wikipedia.org/wiki/Second"),
            new LibUCBase(EnumConversionsTime.microsecond,         "microsecond",          "µs",       mp_micro,                   "https://en.wikipedia.org/wiki/Second"),
            new LibUCBase(EnumConversionsTime.nanosecond,          "nanosecond",           "ns",       mp_nano,                    "https://en.wikipedia.org/wiki/Second"),
            new LibUCBase(EnumConversionsTime.jiffy,               "jiffy",                "j",        mp_one / 60m,               "https://en.wikipedia.org/wiki/Jiffy_(time)"),
            new LibUCBase(EnumConversionsTime.moment,              "moment",               "",         mp_one * 90m,               "https://en.wikipedia.org/wiki/Moment_(time)"),
            new LibUCBase(EnumConversionsTime.minute,              "minute",               "min",      timeMin,                    "https://en.wikipedia.org/wiki/Minute"),
            new LibUCBase(EnumConversionsTime.hour,                "hour",                 "h",        timeHour,                   "https://en.wikipedia.org/wiki/Hour"),
            new LibUCBase(EnumConversionsTime.day,                 "day",                  "d",        timeDay,                    "https://en.wikipedia.org/wiki/Day"),
            new LibUCBase(EnumConversionsTime.week,                "week",                 "wk",       timeDay * 7m,               "https://en.wikipedia.org/wiki/Week"),
            new LibUCBase(EnumConversionsTime.fortnight,           "fortnight",            "fn",       timeDay * 14m,              "https://en.wikipedia.org/wiki/Fortnight"),
            new LibUCBase(EnumConversionsTime.year365,             "year-365d",            "yr 365",   timeDay * 365m,             "https://en.wikipedia.org/wiki/Year"),
            new LibUCBase(EnumConversionsTime.month365,            "month-365d",           "mo 365",   timeDay * 365m / 12m,       "https://en.wikipedia.org/wiki/Month"),
            new LibUCBase(EnumConversionsTime.year366,             "year-366d",            "yr 366",   timeDay * 366m,             "https://en.wikipedia.org/wiki/Year"),
            new LibUCBase(EnumConversionsTime.month366,            "month-366d",           "mo 366",   timeDay * 366m / 12m,       "https://en.wikipedia.org/wiki/Month"),
            new LibUCBase(EnumConversionsTime.yearJulian,          "year-julian",          "yr Jul",   timeDay * 365.25m,          "https://en.wikipedia.org/wiki/Year#Julian_year"),
            new LibUCBase(EnumConversionsTime.monthJulian,         "month-julian",         "mo Jul",   timeDay * 365.25m / 12m,    "https://en.wikipedia.org/wiki/Month#Julian_and_Gregorian_calendars"),
            new LibUCBase(EnumConversionsTime.decadeJulian,        "decade-julian",        "dec Jul",  timeDay * 365.25m * 10m,    "https://en.wikipedia.org/wiki/Century"),
            new LibUCBase(EnumConversionsTime.centuryJulian,       "century-julian",       "c Jul",    timeDay * 365.25m * 100m,   "https://en.wikipedia.org/wiki/Century"),
            new LibUCBase(EnumConversionsTime.millenniumJulian,    "millennium-julian",    "",         timeDay * 365.25m * 1000m,  "https://en.wikipedia.org/wiki/Millennium"),
        };
        #endregion

        /* Conversions Torque **************************************************************************************************/

        #region
        public const string StringHyperlinkTorque = "https://en.wikipedia.org/wiki/Torque";
        private const string StringHyperlinkTorqueDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Torque_or_moment_of_force";

        // Set base unit to first item in list.
        /// <summary>
        /// List of available Torque conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsTorque = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsTorque.newton_meter,              "newton·meter",                 "N·m",      mp_one,                                 "https://en.wikipedia.org/wiki/Newton_metre"),
            new LibUCBase(EnumConversionsTorque.newton_centimeter,         "newton·centimeter",            "N·cm",     mp_one * mp_centi,                      "https://en.wikipedia.org/wiki/Newton_metre"),
            new LibUCBase(EnumConversionsTorque.millinewton_meter,         "millinewton·meter",            "mN·m",     mp_milli,                               "https://en.wikipedia.org/wiki/Newton_metre"),
            new LibUCBase(EnumConversionsTorque.micronewton_meter,         "micronewton·meter",            "µN·m",     mp_micro,                               "https://en.wikipedia.org/wiki/Newton_metre"),
            new LibUCBase(EnumConversionsTorque.kilonewton_meter,          "kilonewton·meter",             "kN·m",     mp_kilo,                                "https://en.wikipedia.org/wiki/Newton_metre"),
            new LibUCBase(EnumConversionsTorque.meganewton_meter,          "meganewton·meter",             "MN·m",     mp_mega,                                "https://en.wikipedia.org/wiki/Newton_metre"),
            new LibUCBase(EnumConversionsTorque.dyne_centimeter,           "dyne·centimeter",              "dyn·cm",   forceDyne * mp_centi,                   ""),
            new LibUCBase(EnumConversionsTorque.kilopond_meter,            "kilopond·meter",               "kp·m",     forceKilogram,                          "https://en.wikipedia.org/wiki/Kilogram-force"),
            new LibUCBase(EnumConversionsTorque.kilogramForce_meter,       "kilogram-force·meter",         "kgf·m",    forceKilogram,                          "https://en.wikipedia.org/wiki/Kilogram-force"),
            new LibUCBase(EnumConversionsTorque.kilogramForce_centimeter,  "kilogram-force·centimeter",    "kgf·cm",   forceKilogram * mp_centi,               "https://en.wikipedia.org/wiki/Kilogram-force"),
            new LibUCBase(EnumConversionsTorque.gramForce_centimeter,      "gram-force·centimeter",        "gf·cm",    forceKilogram * massGram * mp_centi,    ""),
            new LibUCBase(EnumConversionsTorque.ounceForce_inch,           "ounce-force·inch",             "ozf·in",   forceOunce * lengthInch,                ""),
            new LibUCBase(EnumConversionsTorque.ounceForce_foot,           "ounce-force·foot",             "ozf·ft",   forceOunce * lengthFoot,                ""),
            new LibUCBase(EnumConversionsTorque.poundForce_inch,           "pound-force·inch",             "lbf·in",   forcePound * lengthInch,                StringHyperlinkTorqueDefault),
            new LibUCBase(EnumConversionsTorque.poundForce_foot,           "pound-force·foot",             "lbf·ft",   forcePound * lengthFoot,                "https://en.wikipedia.org/wiki/Pound-foot_(torque)"),
            new LibUCBase(EnumConversionsTorque.kipForce_inch,             "kip-force·inch",               "kipf·in",  forcePound * 1000m * lengthInch,        ""),
            new LibUCBase(EnumConversionsTorque.kipForce_foot,             "kip-force·foot",               "kipf·ft",  forcePound * 1000m * lengthFoot,        ""),
            new LibUCBase(EnumConversionsTorque.poundal_inch,              "poundal·inch",                 "pdl·in",   forcePoundal * lengthInch,              StringHyperlinkTorqueDefault),
            new LibUCBase(EnumConversionsTorque.poundal_foot,              "poundal·foot",                 "pdl·ft",   forcePoundal * lengthFoot,              StringHyperlinkTorqueDefault),
        };
        #endregion

        /* Conversions Volume **************************************************************************************************/

        #region
        public const string StringHyperlinkVolume = "https://en.wikipedia.org/wiki/Volume";
        private const string StringHyperlinkVolumeDefault = "https://en.wikipedia.org/wiki/Conversion_of_units#Volume";

        // Set base unit to first item in list.
        /// List of available Volume conversions.
        /// </summary>
        public static readonly List<LibUCBase> listConversionsVolume = new List<LibUCBase>
        {
            new LibUCBase(EnumConversionsVolume.meter_cubed,           "meter³",                   "m³",           mp_one,                                 "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsVolume.decimeter_cubed,       "decimeter³",               "dm³",          mp_deci * mp_deci * mp_deci,            "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsVolume.centimeter_cubed,      "centimeter³",              "cm³",          volumeCentimeterCubed,                  "https://en.wikipedia.org/wiki/Cubic_centimetre"),
            new LibUCBase(EnumConversionsVolume.millimeter_cubed,      "millimeter³",              "mm³",          mp_milli * mp_milli * mp_milli,         "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsVolume.micrometer_cubed,      "micrometer³",              "µm³",          mp_micro * mp_micro * mp_micro,         "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsVolume.nanometer_cubed,       "nanometer³",               "nm³",          mp_nano * mp_nano * mp_nano,            "https://en.wikipedia.org/wiki/Cubic_metre"),
            new LibUCBase(EnumConversionsVolume.liter,                 "liter",                    "L",            volumeLiter,                            "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsVolume.milliliter,            "milliliter",               "mL",           volumeLiter * mp_milli,                 "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsVolume.microliter,            "microliter",               "μL",           volumeLiter * mp_micro,                 "https://en.wikipedia.org/wiki/Litre"),
            new LibUCBase(EnumConversionsVolume.inch_cubed,            "inch³",                    "in³",          volumeInchCubed,                        "https://en.wikipedia.org/wiki/Cubic_inch"),
            new LibUCBase(EnumConversionsVolume.foot_cubed,            "foot³",                    "ft³",          volumeFootCubed,                        "https://en.wikipedia.org/wiki/Cubic_foot"),
            new LibUCBase(EnumConversionsVolume.yard_cubed,            "yard³",                    "yd³",          volumeYardCubed,                        "https://en.wikipedia.org/wiki/Cubic_yard"),
            new LibUCBase(EnumConversionsVolume.boardFoot,             "board-foot",               "fbm",          areaFootSquared * lengthInch,           "https://en.wikipedia.org/wiki/Board_foot"),
            new LibUCBase(EnumConversionsVolume.acre_foot,             "acre·foot",                "ac·ft",        areaMileSquared / 640m * lengthFoot,    "https://en.wikipedia.org/wiki/Acre-foot"),
            new LibUCBase(EnumConversionsVolume.cord,                  "cord",                     "",             volumeFootCubed * 128m,                 "https://en.wikipedia.org/wiki/Wood_fuel#Firewood"),
            new LibUCBase(EnumConversionsVolume.gallonUSfldWine,       "gallon-USfld-Wine",        "gal USf",      volumeGallonUSf,                        "https://en.wikipedia.org/wiki/Gallon"),
            new LibUCBase(EnumConversionsVolume.quartUSfld,            "quart-USfld",              "qt USf",       volumeGallonUSf / 4m,                   "https://en.wikipedia.org/wiki/Quart"),
            new LibUCBase(EnumConversionsVolume.pintUSfld,             "pint-USfld",               "pt USf",       volumeGallonUSf / 8m,                   "https://en.wikipedia.org/wiki/Pint"),
            new LibUCBase(EnumConversionsVolume.cupUSfld,              "cup-USfl",                 "c USf",        volumeGallonUSf / 16m,                  "https://en.wikipedia.org/wiki/Cup_(unit)#United_States"),
            new LibUCBase(EnumConversionsVolume.ounceUSfld,            "ounce-USfld",              "oz USf",       volumeGallonUSf / 128m,                 "https://en.wikipedia.org/wiki/Fluid_ounce"),
            new LibUCBase(EnumConversionsVolume.tablespoonUSfld,       "tablespoon-USfld",         "tbsp USf",     volumeGallonUSf / 256m,                 "https://en.wikipedia.org/wiki/Tablespoon"),
            new LibUCBase(EnumConversionsVolume.teaspoonUSfld,         "teaspoon-USfld",           "tsp USf",      volumeGallonUSf / 768m,                 "https://en.wikipedia.org/wiki/Teaspoon"),
            new LibUCBase(EnumConversionsVolume.dramUSfld,             "dram-USfld",               "dr USf",       volumeGallonUSf / 1024m,                "https://en.wikipedia.org/wiki/Dram_(unit)#Unit_of_volume"),
            new LibUCBase(EnumConversionsVolume.minimUSfld,            "minim-USfld",              "min USf",      volumeGallonUSf / 61440m,               "https://en.wikipedia.org/wiki/Minim_(unit)"),
            new LibUCBase(EnumConversionsVolume.kegUSfldBeer,          "keg-USfld-Beer",           "keg USf",      volumeGallonUSf * 31m,                  "https://en.wikipedia.org/wiki/Keg"),
            new LibUCBase(EnumConversionsVolume.barrelUSfldPetro,      "barrel-USfld-Petro",       "bbl USf",      volumeGallonUSf * 42m,                  "https://en.wikipedia.org/wiki/Barrel_(unit)"),
            new LibUCBase(EnumConversionsVolume.gallonUSdry,           "gallon-USdry",             "gal USd",      volumeGallonUSd,                        "https://en.wikipedia.org/wiki/Gallon#The_US_dry_gallon"),
            new LibUCBase(EnumConversionsVolume.quartUSdry,            "quart-USdry",              "qt USd",       volumeGallonUSd / 4m,                   "https://en.wikipedia.org/wiki/Quart#United_States_dry_quart"),
            new LibUCBase(EnumConversionsVolume.pintUSdry,             "pint-USdry",               "pint USd",     volumeGallonUSd / 8m,                   "https://en.wikipedia.org/wiki/Pint#United_States_dry_pint"),
            new LibUCBase(EnumConversionsVolume.peckUSdry,             "peck-USdry",               "pk USd",       volumeGallonUSd * 2m,                   "https://en.wikipedia.org/wiki/Peck"),
            new LibUCBase(EnumConversionsVolume.bushelUSdry,           "bushel-USdry",             "bu USd",       volumeGallonUSd * 8m,                   "https://en.wikipedia.org/wiki/Bushel"),
            new LibUCBase(EnumConversionsVolume.barrelUSdry,           "barrel-USdry",             "bbl USd",      volumeGallonUSd * 26.25m,               "https://en.wikipedia.org/wiki/Barrel_(unit)#Dry_goods_in_the_US"),
            new LibUCBase(EnumConversionsVolume.gallonIMP,             "gallon-IMP",               "gal IMP",      volumeGallonIMP,                        "https://en.wikipedia.org/wiki/Gallon"),
            new LibUCBase(EnumConversionsVolume.quartIMP,              "quart-IMP",                "qt IMP",       volumeGallonIMP / 4m,                   "https://en.wikipedia.org/wiki/Quart"),
            new LibUCBase(EnumConversionsVolume.pintIMP,               "pint-IMP",                 "pt IMP",       volumeGallonIMP / 8m,                   "https://en.wikipedia.org/wiki/Pint"),
            new LibUCBase(EnumConversionsVolume.gillIMP,               "gill-IMP",                 "gi IMP",       volumeGallonIMP / 32m,                  "https://en.wikipedia.org/wiki/Gill_(unit)"),
            new LibUCBase(EnumConversionsVolume.ounceIMP,              "ounce-IMP",                "oz IMP",       volumeGallonIMP / 160m,                 "https://en.wikipedia.org/wiki/Fluid_ounce"),
            new LibUCBase(EnumConversionsVolume.dramIMP,               "dram-IMP",                 "dr IMP",       volumeGallonIMP / 1280m,                "https://en.wikipedia.org/wiki/Dram_(unit)#Unit_of_volume"),
            new LibUCBase(EnumConversionsVolume.minimIMP,              "minim-IMP",                "min IMP",      volumeGallonIMP / 76800m,               "https://en.wikipedia.org/wiki/Minim_(unit)"),
            new LibUCBase(EnumConversionsVolume.bushelIMP,             "bushel-IMP",               "bu IMP",       volumeGallonIMP * 8m,                   StringHyperlinkVolumeDefault),
            new LibUCBase(EnumConversionsVolume.barrelIMP,             "barrel-IMP",               "bl IMP",       volumeGallonIMP * 36m,                  "https://en.wikipedia.org/wiki/Barrel_(unit)"),
            new LibUCBase(EnumConversionsVolume.cranIMP,               "cran-IMP",                 "",             volumeGallonIMP * 37.5m,                ""),
        };
        #endregion

    }
}
