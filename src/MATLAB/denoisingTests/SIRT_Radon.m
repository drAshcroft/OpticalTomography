

s=size(sinoGram);
nProjections = s(2);

angleStep=360.0/nProjections;

sizeProjections = s(1);

jumpSize =floor( nProjections/4);

reconImageSIRT=reconImage;

R_Thresh = (sizeProjections/2.1)^2;
centerI= sizeProjections/2;
centerJ= sizeProjections/2;
mask=zeros([sizeProjections sizeProjections]);

for I=1:sizeProjections
        for J=1:sizeProjections
            R= ((I-centerI)^2 + (J-centerJ)^2)/R_Thresh;
            if R>1
                mask(I,J)=1;
            end
        end
end
maskIndex = find(mask==1);

tsinoGram = sinoGram;

angleI=1;
figure;
for K=1:15  
   
    reconImageSIRT(maskIndex)=0;
    
    forward=radon(reconImageSIRT ,thetas);
    
    startI=round((size(forward,1)-s(1))/2);
    forward=forward(startI:end-startI,:);
    
    tsinoGram = sinoGram-forward;
    
    addition=iradon(tsinoGram,thetas,'linear','none',1,s(1));
    addition(maskIndex)=0;
    mean(addition(:))
    
    reconImageSIRT = reconImageSIRT +1.0/K/30.0*  addition;
    imagesc(reconImageSIRT);
    drawnow;
end

imagesc(reconImageSIRT(70:end-70,70:end-70));colormap gray;
