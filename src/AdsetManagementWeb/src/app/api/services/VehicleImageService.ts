/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { ImageResponse } from '../models/ImageResponse';
import type { ImageUploadResponse } from '../models/ImageUploadResponse';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
    providedIn: 'root',
})
export class VehicleImageService {
    constructor(public readonly http: HttpClient) {}
    /**
     * @param vehicleId
     * @param formData
     * @returns ImageUploadResponse Success
     * @throws ApiError
     */
    public postApiVehicleImages(
        vehicleId: number,
        formData?: {
            Images?: Array<Blob>;
        },
    ): Observable<ImageUploadResponse> {
        return __request(OpenAPI, this.http, {
            method: 'POST',
            url: '/api/vehicle/{vehicleId}/images',
            path: {
                'vehicleId': vehicleId,
            },
            formData: formData,
            mediaType: 'multipart/form-data',
        });
    }
    /**
     * @param vehicleId
     * @returns ImageResponse Success
     * @throws ApiError
     */
    public getApiVehicleImages(
        vehicleId: number,
    ): Observable<Array<ImageResponse>> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/api/vehicle/{vehicleId}/images',
            path: {
                'vehicleId': vehicleId,
            },
        });
    }
    /**
     * @param vehicleId
     * @param imageId
     * @returns any Success
     * @throws ApiError
     */
    public deleteApiVehicleImages(
        vehicleId: number,
        imageId: number,
    ): Observable<any> {
        return __request(OpenAPI, this.http, {
            method: 'DELETE',
            url: '/api/vehicle/{vehicleId}/images/{imageId}',
            path: {
                'vehicleId': vehicleId,
                'imageId': imageId,
            },
        });
    }
}
