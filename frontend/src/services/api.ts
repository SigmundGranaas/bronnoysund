import axios, { AxiosError } from 'axios';
import type { ProcessedCompaniesResultDto, ApiResponse } from '../types';

const apiClient = axios.create({
    baseURL: 'http://localhost:5150/api',
    headers: {
        'Content-Type': 'multipart/form-data',
    },
});

export const uploadCompanyCsv = async (
    file: File
): Promise<ApiResponse<ProcessedCompaniesResultDto>> => {
    const formData = new FormData();
    formData.append('file', file);

    try {
        const response = await apiClient.post<ProcessedCompaniesResultDto>(
            '/companies/upload',
            formData
        );
        return { data: response.data };
    } catch (error) {
        const axiosError = error as AxiosError;
        if (axiosError.response) {
            const errorData = axiosError.response.data as any;
            return { error: errorData?.message || errorData?.title || axiosError.message || 'An unknown error occurred during upload.' };
        } else {
            return { error: axiosError.message || 'An unknown error occurred' };
        }
    }
};

export const exportCompanyCsv = async (
    file: File
): Promise<ApiResponse<string>> => {
    const formData = new FormData();
    formData.append('file', file);

    try {
        const response = await apiClient.post(
            '/companies/upload/csv',
            formData,
            {
                responseType: 'blob'
            }
        );

        const blob = new Blob([response.data], { type: 'text/csv' });
        const url = URL.createObjectURL(blob);

        return { data: url };
    } catch (error) {
        const axiosError = error as AxiosError;
        if (axiosError.response) {
            const errorData = axiosError.response.data as any;
            return { error: errorData?.message || errorData?.title || axiosError.message || 'An unknown error occurred during export.' };
        } else {
            return { error: axiosError.message || 'An unknown error occurred' };
        }
    }
};