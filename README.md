# EdgeRag
EdgeRag is a Retrieval Augmented Generative (RAG) Artificial Intelligence (AI) pipeline that is based on LLamaSharp and Microsoft's Kernel Memory. It uses Godot 4.2.1 mono and requires .NET8. You can run many popular models (llama, mistral, mixtral, phi) and add nearly any sort of document to the vector database to query. Note: KernelMemory currently does not utilize the GPU properly, so all inference will be slow and CPU-only until this is addressed.

Download a Mistral model from here: https://huggingface.co/TheBloke/Mistral-7B-Instruct-v0.2-GGUF
Download a Phi model from here: https://huggingface.co/TheBloke/phi-2-GGUF
