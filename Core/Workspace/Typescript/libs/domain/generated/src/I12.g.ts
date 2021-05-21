// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { DatabaseObject
, Method } from '@allors/workspace/core';

import { C2 } from './C2.g';
import { I1 } from './I1.g';
import { I2 } from './I2.g';
import { C1 } from './C1.g';
import { Permission } from './Permission.g';

export interface I12 extends DatabaseObject
 {

    CanReadI12AllorsBinary: boolean;
    CanWriteI12AllorsBinary: boolean;
    I12AllorsBinary: any | null;

    CanReadI12C2One2One: boolean;
    CanWriteI12C2One2One: boolean;
    I12C2One2One: C2 | null;

    CanReadI12AllorsDouble: boolean;
    CanWriteI12AllorsDouble: boolean;
    I12AllorsDouble: number | null;

    CanReadI12I1Many2One: boolean;
    CanWriteI12I1Many2One: boolean;
    I12I1Many2One: I1 | null;

    CanReadI12AllorsString: boolean;
    CanWriteI12AllorsString: boolean;
    I12AllorsString: string | null;

    CanReadI12I12Many2Manies: boolean;
    CanWriteI12I12Many2Manies: boolean;
    I12I12Many2Manies: I12[];
    AddI12I12Many2Many(value: I12) : void;
    RemoveI12I12Many2Many(value: I12) : void;

    CanReadI12AllorsDecimal: boolean;
    CanWriteI12AllorsDecimal: boolean;
    I12AllorsDecimal: string | null;

    CanReadI12I2Many2Manies: boolean;
    CanWriteI12I2Many2Manies: boolean;
    I12I2Many2Manies: I2[];
    AddI12I2Many2Many(value: I2) : void;
    RemoveI12I2Many2Many(value: I2) : void;

    CanReadI12C2Many2Manies: boolean;
    CanWriteI12C2Many2Manies: boolean;
    I12C2Many2Manies: C2[];
    AddI12C2Many2Many(value: C2) : void;
    RemoveI12C2Many2Many(value: C2) : void;

    CanReadI12I1Many2Manies: boolean;
    CanWriteI12I1Many2Manies: boolean;
    I12I1Many2Manies: I1[];
    AddI12I1Many2Many(value: I1) : void;
    RemoveI12I1Many2Many(value: I1) : void;

    CanReadI12I12One2Manies: boolean;
    CanWriteI12I12One2Manies: boolean;
    I12I12One2Manies: I12[];
    AddI12I12One2Many(value: I12) : void;
    RemoveI12I12One2Many(value: I12) : void;

    CanReadName: boolean;
    CanWriteName: boolean;
    Name: string | null;

    CanReadI12C1Many2Manies: boolean;
    CanWriteI12C1Many2Manies: boolean;
    I12C1Many2Manies: C1[];
    AddI12C1Many2Many(value: C1) : void;
    RemoveI12C1Many2Many(value: C1) : void;

    CanReadI12I2Many2One: boolean;
    CanWriteI12I2Many2One: boolean;
    I12I2Many2One: I2 | null;

    CanReadI12AllorsUnique: boolean;
    CanWriteI12AllorsUnique: boolean;
    I12AllorsUnique: string | null;

    CanReadI12AllorsInteger: boolean;
    CanWriteI12AllorsInteger: boolean;
    I12AllorsInteger: number | null;

    CanReadI12I1One2Manies: boolean;
    CanWriteI12I1One2Manies: boolean;
    I12I1One2Manies: I1[];
    AddI12I1One2Many(value: I1) : void;
    RemoveI12I1One2Many(value: I1) : void;

    CanReadI12C1One2One: boolean;
    CanWriteI12C1One2One: boolean;
    I12C1One2One: C1 | null;

    CanReadI12I12One2One: boolean;
    CanWriteI12I12One2One: boolean;
    I12I12One2One: I12 | null;

    CanReadI12I2One2One: boolean;
    CanWriteI12I2One2One: boolean;
    I12I2One2One: I2 | null;

    CanReadDependencies: boolean;
    CanWriteDependencies: boolean;
    Dependencies: I12[];
    AddDependency(value: I12) : void;
    RemoveDependency(value: I12) : void;

    CanReadI12I2One2Manies: boolean;
    CanWriteI12I2One2Manies: boolean;
    I12I2One2Manies: I2[];
    AddI12I2One2Many(value: I2) : void;
    RemoveI12I2One2Many(value: I2) : void;

    CanReadI12C2Many2One: boolean;
    CanWriteI12C2Many2One: boolean;
    I12C2Many2One: C2 | null;

    CanReadI12I12Many2One: boolean;
    CanWriteI12I12Many2One: boolean;
    I12I12Many2One: I12 | null;

    CanReadI12AllorsBoolean: boolean;
    CanWriteI12AllorsBoolean: boolean;
    I12AllorsBoolean: boolean | null;

    CanReadI12I1One2One: boolean;
    CanWriteI12I1One2One: boolean;
    I12I1One2One: I1 | null;

    CanReadI12C1One2Manies: boolean;
    CanWriteI12C1One2Manies: boolean;
    I12C1One2Manies: C1[];
    AddI12C1One2Many(value: C1) : void;
    RemoveI12C1One2Many(value: C1) : void;

    CanReadI12C1Many2One: boolean;
    CanWriteI12C1Many2One: boolean;
    I12C1Many2One: C1 | null;

    CanReadI12AllorsDateTime: boolean;
    CanWriteI12AllorsDateTime: boolean;
    I12AllorsDateTime: string | null;



    C1sWhereC1I12Many2Many : C1[];


    C1sWhereC1I12Many2One : C1[];


    C1WhereC1I12One2Many : C1 | null;


    C1WhereC1I12One2One : C1 | null;


    C2sWhereC2I12Many2One : C2[];


    C2WhereC2I12One2One : C2 | null;


    C2sWhereC2I12Many2Many : C2[];


    C2WhereC2I12One2Many : C2 | null;


    I1sWhereI1I12Many2Many : I1[];


    I1sWhereI1I12Many2One : I1[];


    I1WhereI1I12One2One : I1 | null;


    I1WhereI1I12One2Many : I1 | null;


    I12sWhereI12I12Many2Many : I12[];


    I12WhereI12I12One2Many : I12 | null;


    I12WhereI12I12One2One : I12 | null;


    I12sWhereDependency : I12[];


    I12sWhereI12I12Many2One : I12[];


    I2sWhereI2I12Many2One : I2[];


    I2WhereI2I12One2Many : I2 | null;


    I2WhereI2I12One2One : I2 | null;


    I2sWhereI2I12Many2Many : I2[];


}