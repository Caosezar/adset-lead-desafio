/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { OtherOptionsResponse } from './OtherOptionsResponse';
export type VehicleResponse = {
    id?: number;
    imagens?: Array<string> | null;
    marca?: string | null;
    modelo?: string | null;
    ano?: string | null;
    placa?: string | null;
    km?: number | null;
    cor?: string | null;
    preco?: number;
    otherOptions?: OtherOptionsResponse;
    pacoteICarros?: string | null;
    pacoteWebMotors?: string | null;
    creationDate?: string;
    updateDate?: string | null;
    createUserId?: number | null;
    updateUserId?: number | null;
};

