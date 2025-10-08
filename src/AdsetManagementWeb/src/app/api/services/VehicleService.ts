/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { CreateVehicleRequest } from '../models/CreateVehicleRequest';
import type { UpdatePacotesRequest } from '../models/UpdatePacotesRequest';
import type { UpdateVehicleRequest } from '../models/UpdateVehicleRequest';
import type { VehicleListResponse } from '../models/VehicleListResponse';
import type { VehicleResponse } from '../models/VehicleResponse';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
    providedIn: 'root',
})
export class VehicleService {
    constructor(public readonly http: HttpClient) {}
    /**
     * @param requestBody
     * @returns VehicleResponse Success
     * @throws ApiError
     */
    public postApiVehicle(
        requestBody?: CreateVehicleRequest,
    ): Observable<VehicleResponse> {
        return __request(OpenAPI, this.http, {
            method: 'POST',
            url: '/api/Vehicle',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param marca
     * @param modelo
     * @param ano
     * @param cor
     * @param precoMin
     * @param precoMax
     * @param kmMax
     * @param page
     * @param pageSize
     * @returns VehicleListResponse Success
     * @throws ApiError
     */
    public getApiVehicle(
        marca?: string,
        modelo?: string,
        ano?: string,
        cor?: string,
        precoMin?: number,
        precoMax?: number,
        kmMax?: number,
        page?: number,
        pageSize?: number,
    ): Observable<VehicleListResponse> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/api/Vehicle',
            query: {
                'Marca': marca,
                'Modelo': modelo,
                'Ano': ano,
                'Cor': cor,
                'PrecoMin': precoMin,
                'PrecoMax': precoMax,
                'KmMax': kmMax,
                'Page': page,
                'PageSize': pageSize,
            },
        });
    }
    /**
     * @param formData
     * @returns VehicleResponse Success
     * @throws ApiError
     */
    public postApiVehicleWithImages(
        formData?: {
            Marca: string;
            Modelo: string;
            Ano: string;
            Placa: string;
            Km?: number;
            Cor: string;
            Preco: number;
            'OtherOptions.ArCondicionado'?: boolean;
            'OtherOptions.Alarme'?: boolean;
            'OtherOptions.Airbag'?: boolean;
            'OtherOptions.ABS'?: boolean;
            PacoteICarros?: string;
            PacoteWebMotors?: string;
            Images?: Array<Blob>;
        },
    ): Observable<VehicleResponse> {
        return __request(OpenAPI, this.http, {
            method: 'POST',
            url: '/api/Vehicle/with-images',
            formData: formData,
            mediaType: 'multipart/form-data',
        });
    }
    /**
     * @param id
     * @returns VehicleResponse Success
     * @throws ApiError
     */
    public getApiVehicle1(
        id: number,
    ): Observable<VehicleResponse> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/api/Vehicle/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns VehicleResponse Success
     * @throws ApiError
     */
    public putApiVehicle(
        id: number,
        requestBody?: UpdateVehicleRequest,
    ): Observable<VehicleResponse> {
        return __request(OpenAPI, this.http, {
            method: 'PUT',
            url: '/api/Vehicle/{id}',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @returns any Success
     * @throws ApiError
     */
    public deleteApiVehicle(
        id: number,
    ): Observable<any> {
        return __request(OpenAPI, this.http, {
            method: 'DELETE',
            url: '/api/Vehicle/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns VehicleResponse Success
     * @throws ApiError
     */
    public putApiVehiclePacotes(
        id: number,
        requestBody?: UpdatePacotesRequest,
    ): Observable<VehicleResponse> {
        return __request(OpenAPI, this.http, {
            method: 'PUT',
            url: '/api/Vehicle/{id}/pacotes',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
}
