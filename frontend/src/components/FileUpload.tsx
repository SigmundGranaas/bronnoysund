import React, { useState, useCallback } from 'react';
import { useDropzone } from 'react-dropzone';
import CloudDownloadIcon from '@mui/icons-material/CloudDownload';
import FileDownloadIcon from '@mui/icons-material/FileDownload';
import ArticleIcon from '@mui/icons-material/Article';

interface FileUploadProps {
    onFileUpload: (file: File) => void;
    onFileExport: (file: File) => void;
    loading: boolean;
    csvDownloadUrl?: string;
}

const SplitFileUpload: React.FC<FileUploadProps> = 
    ({
        onFileUpload,
        onFileExport,
        loading,
        csvDownloadUrl
    }) => {
    const [activeZone, setActiveZone] = useState<'data' | 'csv'>('data');

    const onDataDrop = useCallback(
        (acceptedFiles: File[]) => {
            setActiveZone('data');
            if (acceptedFiles && acceptedFiles.length > 0 && !loading) {
                onFileUpload(acceptedFiles[0]);
            }
        },
        [onFileUpload, loading]
    );

    const onCsvDrop = useCallback(
        (acceptedFiles: File[]) => {
            setActiveZone('csv');
            if (acceptedFiles && acceptedFiles.length > 0 && !loading) {
                onFileExport(acceptedFiles[0]);
            }
        },
        [onFileExport, loading]
    );

    const dataDropzone = useDropzone({
        onDrop: onDataDrop,
        accept: { 'text/csv': ['.csv'] },
        multiple: false,
        disabled: loading,
    });

    const csvDropzone = useDropzone({
        onDrop: onCsvDrop,
        accept: { 'text/csv': ['.csv'] },
        multiple: false,
        disabled: loading,
    });

    const sharedClasses = `
    p-8
    border-2 border-dashed rounded-xl
    transition-all duration-200 ease-in-out
    flex flex-col items-center justify-center text-center
    min-h-[200px]
    ${loading ? 'opacity-70 cursor-not-allowed' : ''}
  `;

    const dataClasses = `
    ${sharedClasses}
    ${dataDropzone.isDragActive ? 'border-blue-500 bg-blue-900/20' : 'border-dark-border hover:border-blue-400'}
    ${activeZone === 'data' ? 'bg-dark-surface/80' : 'bg-dark-surface/30'}
  `;

    const csvClasses = `
    ${sharedClasses}
    ${csvDropzone.isDragActive ? 'border-purple-500 bg-purple-900/20' : 'border-dark-border hover:border-purple-400'}
    ${activeZone === 'csv' ? 'bg-dark-surface/80' : 'bg-dark-surface/30'}
  `;

    return (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div
                {...dataDropzone.getRootProps()}
                className={dataClasses}
                onClick={() => setActiveZone('data')}
            >
                <input {...dataDropzone.getInputProps()} />
                

                {loading && activeZone === 'data' ? (
                    <LoadingIndicator />
                ) : (
                    <>
                        <ArticleIcon className="mb-3 text-blue-400" style={{ fontSize: '56px' }} />
                        <p className="text-lg font-medium text-dark-text-primary mb-1">
                            Upload CSV for Analysis
                        </p>
                        <p className="text-sm text-dark-text-secondary">
                            Drop here to process company data
                        </p>
                    </>
                )}
            </div>

            <div
                {...csvDropzone.getRootProps()}
                className={csvClasses}
                onClick={() => setActiveZone('csv')}
            >
                <input {...csvDropzone.getInputProps()} />
                

                {loading && activeZone === 'csv' ? (
                    <LoadingIndicator />
                ) : (
                    <>
                        <CloudDownloadIcon className="mb-3 text-purple-400" style={{ fontSize: '56px' }} />
                        <p className="text-lg font-medium text-dark-text-primary mb-1">
                            Process & Download CSV
                        </p>
                        <p className="text-sm text-dark-text-secondary">
                            Drop here to download transformed CSV data
                        </p>
                        {csvDownloadUrl && (
                            <a
                                href={csvDownloadUrl}
                                download="firmaer_output.csv"
                                className="mt-4 flex items-center gap-2 px-3 py-1 bg-purple-700 hover:bg-purple-600 transition-colors rounded text-white"
                                onClick={(e) => e.stopPropagation()}
                            >
                                <FileDownloadIcon fontSize="small" />
                                Download CSV
                            </a>
                        )}
                    </>
                )}
            </div>
        </div>
    );
};

const LoadingIndicator = () => (
    <svg className="animate-spin h-10 w-10 text-blue-400" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
    </svg>
);

export default SplitFileUpload;