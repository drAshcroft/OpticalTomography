NET.addAssembly('C:\Development\CellCT\Tomographic_Imaging - Current\Tomography Simplified_New\bin\DebugDLL\ReconstructCells.dll');

Directories = ReconstructCells.Tools.MatlabHelps.GetPaperDirectories('V:\\ASU_Recon\\viveks_RT10','brightimages')


s=100;
origPSF=zeros([s s]);
for I=1:s/2-1
    radius = I;
    
    padded =zeros([s s]);
    for J=1:s
        for K=1:s
            R=((J-s/2)^2+(K-s/2)^2)^.5;
            if R<radius
                padded(J,K)=1;
            end
        end
    end
    origPSF=origPSF+padded/I^2;
end
h = fspecial('gaussian', 35, 5);
origPSF2=conv2(origPSF,h,'same');


% origPSF2=origPSF;
sumf = sum(origPSF2(:));
origPSF2 = origPSF2/sumf;

for dirI=1:1%Directories.Length
    dir =char( Directories(dirI));
    
    lib = ReconstructCells.Tools.MatlabHelps.GetLibraryForMatlab([dir '\\brightimages']);
    psf=[];
    tpsf=[];
    image =double( lib.Item(1).Data);
    sinoGram=zeros([500 size(image,1)]);
    h=floor(size(image,1)/2-25);
    for I=1:lib.Count
        image =double( lib.Item(I-1).Data);
        %image = image - imfilter(image,h);
        sinoGram(I,:)=image(:,h);
        x=floor(40*sin(double(I)/500*3.1415*2-15/180*3.1415)+h+25);
        sinoGram(I,x-1:x+1)=min(image(:));
    end
    graymap(sinoGram);
end