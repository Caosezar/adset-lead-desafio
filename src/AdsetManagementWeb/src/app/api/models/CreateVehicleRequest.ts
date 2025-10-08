/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { OtherOptionsRequest } from './OtherOptionsRequest';
export type CreateVehicleRequest = {
    imagens?: Array<string> | null;
    marca: string;
    modelo: string;
    ano: string;
    placa: string;
    km?: number | null;
    cor: string;
    preco: number;
    otherOptions?: OtherOptionsRequest;
    pacoteICarros?: string | null;
    pacoteWebMotors?: string | null;
};

