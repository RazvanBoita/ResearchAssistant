import { useState, useEffect, useRef } from 'react';
import { FileText, Upload, BookOpen, Key, Activity, AlertCircle, CheckCircle, X, ChevronDown } from 'lucide-react';
import './AIResearchAssistant.css';

export default function AIResearchAssistant() {
  const [files, setFiles] = useState([]);
  const [isProcessing, setIsProcessing] = useState(false);
  const [result, setResult] = useState(null);
  const [displayedText, setDisplayedText] = useState("");
  const [isStreaming, setIsStreaming] = useState(false);
  const [streamComplete, setStreamComplete] = useState(false);
  const [analysisType, setAnalysisType] = useState('summary');
  const [error, setError] = useState(null);
  const [models, setModels] = useState([]);
  const [selectedModel, setSelectedModel] = useState("");
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [streamingText, setStreamingText] = useState("");

  const baseUrl = "http://localhost:5283"; // Replace with your actual base URL
  const dropdownRef = useRef(null);

  // Handle click outside to close dropdown
  useEffect(() => {
    function handleClickOutside(event) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setIsDropdownOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, []);

  // Fetch models from the backend
  useEffect(() => {
    const fetchModels = async () => {
      try {
        const response = await fetch(`${baseUrl}/api/models`);
        if (!response.ok) {
          throw new Error("Failed to fetch models");
        }
        const data = await response.json();
        setModels(data);
        if (data.length > 0) {
          setSelectedModel(data[0]); // Default to the first model
        }
      } catch (err) {
        setError("Failed to load models: " + err.message);
      }
    };

    fetchModels();
  }, []);

  // Simplified streaming effect for the response text
  useEffect(() => {
    if (result && displayedText) {
      setIsStreaming(true);
      setStreamingText("");
      
      let index = 0;
      const textLength = displayedText.length;
      
      const interval = setInterval(() => {
        if (index < textLength) {
          setStreamingText(current => current + displayedText.charAt(index));
          index++;
        } else {
          setIsStreaming(false);
          setStreamComplete(true);
          clearInterval(interval);
        }
      }, 15); // Slightly faster for better effect
      
      return () => clearInterval(interval);
    }
  }, [result, displayedText]);

  const handleFileChange = (e) => {
    const fileList = Array.from(e.target.files);
    setFiles(prev => [...prev, ...fileList]);
  };

  const removeFile = (index) => {
    setFiles(files.filter((_, i) => i !== index));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (files.length === 0) {
      setError("Please upload at least one document");
      return;
    }
    if (!selectedModel) {
      setError("Please select a model");
      return;
    }

    setIsProcessing(true);
    setError(null);
    setResult(null);
    setStreamComplete(false);
    setStreamingText("");

    const formData = new FormData();
    files.forEach(file => formData.append("file", file));

    try {
      // Different API endpoints based on analysis type
      const endpoint = analysisType === 'summary' 
        ? `${baseUrl}/api/summary?model=${selectedModel}`
        : `${baseUrl}/api/insights?model=${selectedModel}`;
      
      // Make the actual API call to generate summary or insights
      const response = await fetch(endpoint, {
        method: "POST",
        body: formData,
      });

      if (!response.ok) {
        throw new Error(`Failed to generate ${analysisType}`);
      }

      const data = await response.text();
      setResult({ type: analysisType });
      setDisplayedText(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setIsProcessing(false);
    }
  };

  const toggleDropdown = () => {
    if (!isProcessing && !isStreaming) {
      setIsDropdownOpen(!isDropdownOpen);
    }
  };

  const selectModel = (model) => {
    setSelectedModel(model);
    setIsDropdownOpen(false);
  };

  const resultRef = useRef(null);

  useEffect(() => {
    if (resultRef.current && isStreaming) {
      resultRef.current.scrollTop = resultRef.current.scrollHeight;
    }
  }, [streamingText, isStreaming]);

  return (
    <div className="app-container">
      <div className="app-card">
        <div className="app-card-content">
          <div className="header">
            <Activity className="header-icon" />
            <h1 className="header-title">AI Research Assistant</h1>
          </div>

          <p className="description">
            Upload your research documents and our AI will analyze them to generate a summary or extract key insights.
          </p>

          <div className="analysis-options">
            <div className="button-group">
              <button
                onClick={() => setAnalysisType('summary')}
                className={`option-button ${analysisType === 'summary' ? 'active' : ''}`}
                disabled={isProcessing || isStreaming}
              >
                <BookOpen className="button-icon" />
                Generate Summary
              </button>

              <button
                onClick={() => setAnalysisType('insights')}
                className={`option-button ${analysisType === 'insights' ? 'active' : ''}`}
                disabled={isProcessing || isStreaming}
              >
                <Key className="button-icon" />
                Extract Key Insights
              </button>
            </div>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label className="form-label">Select Model</label>
              <div className="custom-dropdown" ref={dropdownRef}>
                <button 
                  type="button"
                  className="dropdown-button"
                  onClick={toggleDropdown}
                  disabled={isProcessing || isStreaming || models.length === 0}
                >
                  <span className="selected-option">{selectedModel || "Select a model"}</span>
                  <ChevronDown className={`dropdown-icon ${isDropdownOpen ? 'rotate' : ''}`} />
                </button>
                
                {isDropdownOpen && (
                  <div className="dropdown-menu">
                    {models.map((model, index) => (
                      <div 
                        key={index} 
                        className={`dropdown-item ${selectedModel === model ? 'selected' : ''}`}
                        onClick={() => selectModel(model)}
                      >
                        {model}
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>

            <div className="form-group">
              <label className="form-label">Upload Documents</label>
              <div className="upload-area">
                <input
                  type="file"
                  multiple
                  onChange={handleFileChange}
                  className="file-input"
                  id="file-upload"
                  accept=".pdf,.doc,.docx,.txt"
                  disabled={isProcessing || isStreaming}
                />
                <label htmlFor="file-upload" className="upload-label">
                  <div className="upload-content">
                    <Upload className="upload-icon" />
                    <p className="upload-text">
                      Drag and drop files here or click to browse
                    </p>
                    <p className="upload-subtext">
                      Supports PDF, DOC, DOCX, TXT
                    </p>
                  </div>
                </label>
              </div>
            </div>

            {files.length > 0 && (
              <div className="form-group">
                <h3 className="files-heading">Uploaded Files</h3>
                <ul className="files-list">
                  {files.map((file, index) => (
                    <li key={index} className="file-item">
                      <div className="file-info">
                        <FileText className="file-icon" />
                        <span className="file-name">{file.name}</span>
                      </div>
                      <button
                        type="button"
                        onClick={() => removeFile(index)}
                        className="remove-button"
                        disabled={isProcessing || isStreaming}
                      >
                        <X className="remove-icon" />
                      </button>
                    </li>
                  ))}
                </ul>
              </div>
            )}

            {error && (
              <div className="error-container">
                <div className="error-content">
                  <AlertCircle className="error-icon" />
                  <span className="error-message">{error}</span>
                </div>
              </div>
            )}

            <button
              type="submit"
              disabled={isProcessing || isStreaming || files.length === 0 || !selectedModel}
              className={`submit-button ${isProcessing || isStreaming ? 'processing' : ''}`}
            >
              {isProcessing ? 'Processing Documents...' : isStreaming ? 'Generating Results...' : `Analyze Documents (${analysisType === 'summary' ? 'Summary' : 'Key Insights'})`}
            </button>
          </form>

          {result && (
            <div className="result-container" ref={resultRef}>
              <div className="result-header">
                <CheckCircle className="success-icon" />
                <h3 className="result-title">
                  {result.type === 'summary' ? 'Document Summary' : 'Key Insights'}
                </h3>
              </div>

              <div className="summary-content">
                <div className="streaming-text">
                  {isStreaming ? streamingText : displayedText}
                  {isStreaming && <span className="cursor-blink">|</span>}
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}